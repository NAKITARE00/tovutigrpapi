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
                var result = await connection.ExecuteAsync(sql, new
                {
                    user.Name
                });
                if (result > 0)
                {
                    return "User added successfully.";
                }
                else
                {
                    return "Failed to add user.";
                }
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
    }
}
