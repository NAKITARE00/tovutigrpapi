using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Repositories
{
    public class IssueCategoryRepository : IIssues
    {
        private readonly DataContext _dataContext;

        public IssueCategoryRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<string> AddIssue(Issues issues)
        {
            string sql = @"
                INSERT INTO Issues (Name, Issue, Gadget_Id)
                VALUES (@Name, @Issue, @Gadget_Id);
            ";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    issues.Name,
                    issues.Issue,
                    issues.Gadget_Id
                });
                return result > 0 ? "Issue added successfully." : "Failed to add issue.";
            }
        }
        public async Task<IEnumerable<Issues>> GetAllIssues()
        {
            string sql = "SELECT * FROM Issues";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Issues>(sql);
            }
        }

        public async Task<IssueRetrieval> GetSingleIssue(int issueId)
        {
            string sql = @"
                SELECT 
                    i.Id AS IssueId,
                    i.Name,
                    i.Issue,
                    i.Gadget_Id,
                    g.Name AS GadgetName
                FROM Issues i
                JOIN Gadgets g ON i.Gadget_Id = g.Id
                WHERE i.Id = @IssueId;
            ";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<IssueRetrieval>(sql, new { IssueId = issueId });
            }
        }

        public async Task<string> UpdateIssue(Issues issues)
        {
            string sql = @"
                UPDATE Issues
                SET Name = @Name,
                    Issue = @Issue,
                    Gadget_Id = @Gadget_Id
                WHERE Id = @Id;
            ";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    issues.Id,
                    issues.Name,
                    issues.Issue,
                    issues.Gadget_Id
                });
                return result > 0 ? "Issue updated successfully." : "Issue not found or update failed.";
            }
        }

        public async Task<string> DeleteIssue(int issueId)
        {
            string sql = "DELETE FROM Issues WHERE Id = @Id;";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { Id = issueId });
                return result > 0 ? "Issue deleted successfully." : "Issue not found or could not be deleted.";
            }
        }
    }
}
