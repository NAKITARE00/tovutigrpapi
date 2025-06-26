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
            string sql = @"
                INSERT INTO Staff (Name)
                VALUES (@Name);
            ";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { user.Name });
                return result > 0 ? "User added successfully." : "Failed to add user.";
            }
        }

        public async Task<IEnumerable<Users>> GetAllUsers()
        {
            string sql = "SELECT * FROM Staff";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Users>(sql);
            }
        }

        public async Task<Users> GetSingleUser(int userId)
        {
            string sql = @"
                SELECT Id, Name
                FROM Staff
                WHERE Id = @UserId;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<Users>(sql, new { UserId = userId });
            }
        }

        public async Task<string> UpdateUser(Users user)
        {
            string sql = @"
                UPDATE Staff
                SET Name = @Name
                WHERE Id = @Id;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { user.Id, user.Name });
                return result > 0 ? "User updated successfully." : "User not found or update failed.";
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
    }
}

