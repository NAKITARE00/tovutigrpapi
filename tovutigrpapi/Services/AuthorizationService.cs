using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Models;

namespace tovutigrpapi.Services
{
    public class AuthorizationService
    {
        private readonly DataContext _dataContext;

        public AuthorizationService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<UsersRetrieval> GetUserById(int staffId)
        {
            string sql = @"
                SELECT
                    s.Id,
                    s.Name,
                    s.Email,
                    s.RoleId,
                    r.UserType,
                    r.RoleName,
                    s.Client_Id AS ClientId,
                    s.Station_Id AS StationId,
                    c.Name  AS ClientName,
                    st.Name AS StationName,
                    st.Location AS StationLocation
                FROM Staff s
                LEFT JOIN Clients c ON c.Id = s.Client_Id
                LEFT JOIN Station st ON st.Id = s.Station_Id
                LEFT JOIN Roles r ON r.RoleId = s.RoleId
                WHERE s.Id = @StaffId;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<UsersRetrieval>(sql, new { StaffId = staffId });
            }
        }

        public async Task<(string UserType, string RoleName, int StaffId, int ClientId, int StationId)> GetUserRole(int staffId)
        {
            var user = await GetUserById(staffId);

            if (user == null)
                throw new UnauthorizedAccessException("Staff not found.");

            return (user.UserType, user.RoleName, user.Id, user.ClientId, user.StationId);
        }
    }
}
