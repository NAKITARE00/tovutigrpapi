using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Services;

namespace tovutigrpapi.Repositories
{
    public class IssueCategoryRepository : IIssues
    {
        private readonly DataContext _dataContext;
        private readonly AuthorizationService _authorizationService;

        public IssueCategoryRepository(DataContext dataContext, AuthorizationService authorizationService)
        {
            _dataContext = dataContext;
            _authorizationService = authorizationService;
        }

        public async Task<string> AddIssue(Issues issues, int staff_id)
        {
            var (userType, roleName, staffId, clientId, stationId) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager")
                throw new UnauthorizedAccessException("User role not allowed to add issues.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                if (roleName == "Manager")
                {
                    string validateSql = @"
                SELECT COUNT(*)
                FROM Stations s
                INNER JOIN Gadgets g ON g.Station_Id = s.Id
                WHERE g.Id = @GadgetId AND s.Client_Id = @ClientId";

                    int count = await connection.ExecuteScalarAsync<int>(validateSql, new
                    {
                        GadgetId = issues.Gadget_Id,
                        ClientId = clientId
                    });

                    if (count == 0)
                        throw new UnauthorizedAccessException("Manager not authorized to add issue for this gadget/station.");
                }

                string sql = @"
            INSERT INTO Issues (Name, Issue, Gadget_Id, Recommendations, Status, Station_Id)
            VALUES (@Name, @Issue, @Gadget_Id, @Recommendations, @Status, @Station_Id);
            ";

                var result = await connection.ExecuteAsync(sql, new
                {
                    issues.Name,
                    issues.Issue,
                    issues.Gadget_Id,
                    issues.Recommendations,
                    issues.Status,
                    issues.Station_Id
                });

                return result > 0 ? "Issue added successfully." : "Failed to add issue.";
            }
        }

        public async Task<IEnumerable<Issues>> GetAllIssues(int staff_id)
        {
            var (userType, roleName, staffId, clientId, stationId) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                string sql;

                if (roleName == "Admin")
                {
                    sql = @"
                SELECT i.* 
                FROM Issues i
                INNER JOIN Gadgets g ON i.Gadget_Id = g.Id
                INNER JOIN Station s ON g.Station_Id = s.Id";

                    return await connection.QueryAsync<Issues>(sql);
                }
                else if (userType == "Administrator" && roleName == "Manager")
                {
                    sql = @"
                SELECT i.* 
                FROM Issues i
                INNER JOIN Gadgets g ON i.Gadget_Id = g.Id
                INNER JOIN Station s ON g.Station_Id = s.Id
                WHERE s.Client_Id = @ClientId";

                    return await connection.QueryAsync<Issues>(sql, new { ClientId = clientId });
                }
                //Normal Role & UserType Supervisor
                else
                {
                    sql = @"
                SELECT i.* 
                FROM Issues i
                INNER JOIN Gadgets g ON i.Gadget_Id = g.Id
                WHERE g.Station_Id = @StationId";

                    return await connection.QueryAsync<Issues>(sql, new { StationId = stationId });
                }
            }
        }

        public async Task<IssueRetrieval> GetSingleIssue(int issueId, int staff_id)
        {
            var (userType, roleName, staffId, clientId, stationId) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                string sql = @"SELECT i.Id, i.Name, i.Issue, i.Gadget_Id, i.Recommendations, i.Status, i.Station_Id,
                              g.Name AS Gadget_Name, st.Name AS Station_Name
                       FROM Issues i
                       LEFT JOIN Gadgets g ON i.Gadget_Id = g.Id
                       LEFT JOIN Station st ON i.Station_Id = st.Id
                       WHERE i.Id = @IssueId;";

                string clientSql = @"SELECT st.Client_Id
                             FROM Issues i
                             LEFT JOIN Station st ON i.Station_Id = st.Id
                             WHERE i.Id = @IssueId;";

                var issue = await connection.QueryFirstOrDefaultAsync<IssueRetrieval>(sql, new { IssueId = issueId });
                var issueClientId = await connection.QueryFirstOrDefaultAsync<int?>(clientSql, new { IssueId = issueId });

                if (issue == null)
                    throw new KeyNotFoundException("Issue not found.");

                if (userType == "Administrator" && roleName == "Manager" && issueClientId != clientId)
                    throw new UnauthorizedAccessException("Manager not authorized to view this issue.");
                if ((roleName == "Normal" || userType == "Supervisor")  && issue.Station_Id != stationId)
                    throw new UnauthorizedAccessException("User not authorized to view this issue.");

                return issue;
            }
        }
        public async Task<string> UpdateIssue(Issues issues, int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");
            if (roleName != "Admin" && roleName != "Manager")
                throw new UnauthorizedAccessException("You are not authorized to update issues.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                // Get the issue's client for authorization
                var issueClientId = await connection.ExecuteScalarAsync<int?>(
                    "SELECT s.Client_Id FROM Issues i JOIN Station s ON i.Station_Id = s.Id WHERE i.Id = @IssueId;",
                    new { IssueId = issues.Id });
                if (issueClientId == null)
                    throw new KeyNotFoundException("Issue not found.");

                if (userType == "Administrator" && roleName == "Manager" && issueClientId != clientId)
                    throw new UnauthorizedAccessException("You are not authorized to update this issue.");
                if (userType == "Supervisor" && issues.Station_Id != station_id)
                    throw new UnauthorizedAccessException("You are not authorized to update this issue.");

                // Update query
                var result = await connection.ExecuteAsync(
                    "UPDATE Issues SET Name=@Name, Issue=@Issue, Gadget_Id=@Gadget_Id, Recommendations=@Recommendations, Status=@Status, Station_Id=@Station_Id WHERE Id=@Id;",
                    new
                    {
                        issues.Id,
                        issues.Name,
                        issues.Issue,
                        issues.Recommendations,
                        issues.Gadget_Id,
                        issues.Status,
                        issues.Station_Id
                    });
                return result > 0 ? "Issue updated successfully." : "Issue not found or update failed.";
            }
        }
        public async Task<string> DeleteIssue(int issueId, int staff_id)
        {
            var (_, roleName, _, _, _) = await _authorizationService.GetUserRole(staff_id);
            if (roleName != "Admin")
                throw new UnauthorizedAccessException("Only admins can delete issues.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync("DELETE FROM Issues WHERE Id=@Id;", new { Id = issueId });
                return result > 0 ? "Issue deleted successfully." : "Issue not found or could not be deleted.";
            }
        }

        public async Task<IEnumerable<Issues>> GetAllIssuesAnalytics()
        {
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                string sql = @"
                SELECT i.* 
                FROM Issues i
                INNER JOIN Gadgets g ON i.Gadget_Id = g.Id
                INNER JOIN Station s ON g.Station_Id = s.Id";

                return await connection.QueryAsync<Issues>(sql);
            }

        }
    }
}
