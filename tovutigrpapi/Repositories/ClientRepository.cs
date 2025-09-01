using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Services;

namespace tovutigrpapi.Repositories
{
    public class ClientRepository : IClients
    {
        private readonly DataContext _dataContext;
        private readonly AuthorizationService _authorizationService;

        public ClientRepository(DataContext dataContext, AuthorizationService authorizationService)
        {
            _dataContext = dataContext;
            _authorizationService = authorizationService;
        }

        public async Task<IEnumerable<Client>> GetAllClients(int staff_id)
        {
            var (userType, roleName, staffId, clientId, _) = await _authorizationService.GetUserRole(staff_id);

            if (userType != "Administrator")
                throw new UnauthorizedAccessException("Only Administrators can access clients.");

            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                if (roleName == "Normal")
                {
                    throw new UnauthorizedAccessException("Normal Administrators are not allowed to access clients.");
                }
                else if (roleName == "Manager")
                {
                    string sql = "SELECT * FROM Clients WHERE Id = @clientId";
                    return await conn.QueryAsync<Client>(sql, new { clientId });
                }
                else if (roleName == "Admin")
                {
                    string sql = "SELECT * FROM Clients";
                    return await conn.QueryAsync<Client>(sql);
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid administrator role.");
                }
            }
        }
        public async Task<Client> GetSingleClient(int id, int staff_id)
        {
            var (userType, roleName, staffId, clientId, _) = await _authorizationService.GetUserRole(staff_id);

            if (userType != "Administrator")
                throw new UnauthorizedAccessException("Only Administrators can access clients.");

            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                if (roleName == "Normal")
                {
                    throw new UnauthorizedAccessException("Normal Administrators are not allowed to access clients.");
                }
                else if (roleName == "Manager")
                {
                    string sql = "SELECT * FROM Clients WHERE Id = @Id AND Id = @clientId";
                    return await conn.QueryFirstOrDefaultAsync<Client>(sql, new { Id = id, clientId });
                }
                else if (roleName == "Admin")
                {
                    string sql = "SELECT * FROM Clients WHERE Id = @Id";
                    return await conn.QueryFirstOrDefaultAsync<Client>(sql, new { Id = id });
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid administrator role.");
                }
            }
        }

        public async Task<string> AddClient(Client client, int staff_id)
        {
            var (userType, roleName, staffId, clientId, _) = await _authorizationService.GetUserRole(staff_id);

            if (userType != "Administrator")
                throw new UnauthorizedAccessException("Only Administrators can add clients.");

            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                if (roleName == "Normal")
                {
                    throw new UnauthorizedAccessException("Normal Administrators are not allowed to add clients.");
                }
                else if (roleName == "Manager")
                {
                    if (client.Id != clientId)
                        throw new UnauthorizedAccessException("Managers can only add clients under their assigned client_id.");
                }

                string sql = @"INSERT INTO Clients (Name, Country) VALUES (@Name, @Country)";
                var result = await conn.ExecuteAsync(sql, client);
                return result > 0 ? "Client added successfully." : "Failed to add client.";
            }
        }

        public async Task<string> UpdateClient(Client client, int staff_id)
        {
            var (userType, roleName, staffId, clientId, _) = await _authorizationService.GetUserRole(staff_id);

            if (userType != "Administrator")
                throw new UnauthorizedAccessException("Only Administrators can update clients.");

            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                if (roleName == "Normal")
                {
                    throw new UnauthorizedAccessException("Normal Administrators are not allowed to update clients.");
                }
                else if (roleName == "Manager" && client.Id != clientId)
                {
                    throw new UnauthorizedAccessException("Managers can only update their assigned client.");
                }

                string sql = @"UPDATE Clients SET Name = @Name, Country = @Country WHERE Id = @Id";
                var result = await conn.ExecuteAsync(sql, client);
                return result > 0 ? "Client updated successfully." : "Failed to update client.";
            }
        }

        public async Task<string> DeleteClient(int id, int staff_id)
        {
            var (userType, roleName, staffId, clientId, _) = await _authorizationService.GetUserRole(staff_id);

            if (userType != "Administrator")
                throw new UnauthorizedAccessException("Only Administrators can delete clients.");

            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                if (roleName == "Normal" || roleName == "Manager")
                {
                    throw new UnauthorizedAccessException("Not allowed to delete clients.");
                }
                string sql = "DELETE FROM Clients WHERE Id = @Id";
                var result = await conn.ExecuteAsync(sql, new { Id = id });
                return result > 0 ? "Client deleted successfully." : "Client not found or already deleted.";
            }
        }


        public async Task<IEnumerable<Client>> GetAllClientsAnalytics()
        {
            string sql = "SELECT * FROM Clients";
            using (IDbConnection conn = _dataContext.CreateConnection())
            {
                return await conn.QueryAsync<Client>(sql);
            }
        }
    }
}
