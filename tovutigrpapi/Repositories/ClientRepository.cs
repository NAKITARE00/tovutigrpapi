using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Repositories
{
    public class ClientRepository : IClients
    {
        private readonly DataContext _dataContext;

        public ClientRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<Client>> GetAllClients()
        {
            string sql = "SELECT * FROM Clients";
            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                return await conn.QueryAsync<Client>(sql);
            }
        }

        public async Task<Client> GetSingleClient(int id)
        {
            string sql = "SELECT * FROM Clients WHERE Id = @Id";
            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<Client>(sql, new { Id = id });
            }
        }

        public async Task<string> AddClient(Client client)
        {
            string sql = @"
                INSERT INTO Clients (Name, Country)
                VALUES (@Name, @Country)";
            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                var result = await conn.ExecuteAsync(sql, client);
                return result > 0 ? "Client added successfully." : "Failed to add client.";
            }
        }

        public async Task<string> UpdateClient(Client client)
        {
            string sql = @"
                UPDATE Clients
                SET Name = @Name,
                    Country = @Country
                WHERE Id = @Id";
            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                var result = await conn.ExecuteAsync(sql, client);
                return result > 0 ? "Client updated successfully." : "Failed to update client.";
            }
        }

        public async Task<string> DeleteClient(int id)
        {
            string sql = "DELETE FROM Clients WHERE Id = @Id";
            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                var result = await conn.ExecuteAsync(sql, new { Id = id });
                return result > 0 ? "Client deleted successfully." : "Client not found or already deleted.";
            }
        }
    }
}
