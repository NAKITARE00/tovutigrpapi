using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Repositories
{
    public class UserRepository : IUsers
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<string> AddUser(Users user)
        {
            if (string.IsNullOrWhiteSpace(user.HashPassword))
                return "Password is required.";

            var (hashedPassword, salt) = PasswordHelper.HashPassword(user.HashPassword);

            string sql = @"
        INSERT INTO Staff (Name, HashPassword, Salt, Email, RoleId, Client_Id, Station_Id)
        VALUES (@Name, @HashPassword, @Salt, @Email, @RoleId, @ClientId, @StationId);
        ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    user.Name,
                    HashPassword = hashedPassword,
                    Salt = salt,
                    user.Email,
                    user.RoleId,
                    user.ClientId,
                    user.StationId
                });

                return result > 0 ? "User added successfully." : "Failed to add user.";
            }
        }

        public async Task<string> UpdateUser(Users user)
        {
            string sql = @"
        UPDATE Staff
        SET Name = @Name,
            Role = @RoleId,
            Email = @Email,
            Client_Id = @ClientId,
            Station_Id = @StationId
        WHERE Id = @Id;
    ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    user.Id,
                    user.Name,
                    user.RoleId,
                    user.Email,
                    user.ClientId,
                    user.StationId
                });

                return result > 0 ? "User updated successfully." : "User not found or update failed.";
            }
        }

        public async Task<IEnumerable<UsersRetrieval>> GetAllUsers()
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
        ORDER BY s.Id DESC";


            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<UsersRetrieval>(sql);
            }
        }

        public async Task<UsersRetrieval?> GetSingleUser(int userId)
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
        WHERE s.Id = @UserId;

        ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<UsersRetrieval>(sql, new { UserId = userId });
            }
        }

        public async Task<UsersRetrieval?> GetUserByEmail(string email)
        {
            string sql = @"
        SELECT
            s.Id, s.Name, s.HashPassword, s.Salt, s.Email, s.RoleId, r.UserType, r.RoleName, 
            s.Client_Id AS ClientId,s.Station_Id AS StationId, c.Name  AS ClientName,st.Name AS StationName, st.Location AS StationLocation
        FROM Staff s
        LEFT JOIN Clients c ON c.Id = s.Client_Id
        LEFT JOIN Station st ON st.Id = s.Station_Id
        LEFT JOIN Roles r ON r.RoleId = s.RoleId
        WHERE s.Email = @Email;
        ";
            
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<UsersRetrieval>(sql, new { Email = email });
            }
        }
        public async Task<string> DeleteUser(int userId)
        {
            string sql = "DELETE FROM Staff WHERE Id = @UserId;";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { UserId = userId });
                return result > 0 ? "User deleted successfully." : "User not found or could not be deleted.";
            }
        }

        public async Task<IEnumerable<Role>> GetAllRoles()
        {
            string sql = "SELECT RoleId, UserType, RoleName FROM Roles;";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Role>(sql);
            }
        }
    }
}

