using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Services;

namespace tovutigrpapi.Repositories
{
    public class StationRepository : IStations
    {
        private readonly DataContext _dataContext;
        private readonly AuthorizationService _authorizationService;

        public StationRepository(DataContext dataContext, AuthorizationService authorizationService)
        {
            _dataContext = dataContext;
            _authorizationService = authorizationService;
        }

        public async Task<string> AddStation(Stations station, int staff_id)
        {
            var (_, roleName, _, _, _) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            if (!roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return "Access denied. Only admins can add stations.";

            const string sql = @"INSERT INTO Station (Name, Station, Location, Client_Id, Status) 
                         VALUES (@Name, @Station, @Location, @Client_Id, @Status);";

            using (var connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    station.Name,
                    station.Station,
                    station.Location,
                    station.Client_Id,
                    station.Status
                });

                return result > 0 ? "Station added successfully." : "Failed to add station.";
            }
        }

        public async Task<IEnumerable<Stations>> GetAllStations(int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            string sql = string.Empty;
            object parameters = null;

            if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                sql = "SELECT * FROM Station";
            }
            else if (userType == "Administrator" &&  roleName == "Manager" )
            {
                sql = "SELECT * FROM Station WHERE Client_Id = @ClientId";
                parameters = new { ClientId = clientId };
            }
            else if (userType == "Supervisor" && roleName == "Manager")
            {
                sql = "SELECT * FROM Station WHERE Id = @StationId";
                parameters = new { ClientId = clientId };
            }
            else if (roleName.Equals("Normal", StringComparison.OrdinalIgnoreCase))
            {
                sql = "SELECT * FROM Station WHERE Client_Id = @ClientId AND Id = @StationId";
                parameters = new { ClientId = clientId, StationId = station_id };
            }
            else
            {
                return Enumerable.Empty<Stations>();
            }

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Stations>(sql, parameters);
            }
        }
        public async Task<IEnumerable<StationRetrieval>> GetSingleStation(int stationId, int staff_id)
        {
            var (userType, roleName, _, clientId, _) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            string sql = "SELECT s.Id, s.Name, s.Station, s.Location, s.Client_Id, s.Date_Created, s.Status, c.Name AS ClientName, c.Country AS ClientCountry " +
                         "FROM Station s LEFT JOIN Clients c ON s.Client_Id = c.Id WHERE s.Id = @StationId";

            if ( userType == "Administrator" && roleName == "Manager")
            {
                sql += " AND s.Client_Id = @ClientId";
            }
            else if (roleName == "Normal" || userType == "Supervisor")
            {
                sql += " AND s.Id = @StationId"; 
            }
       
            else if (!roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("User role not recognized.");
            }
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<StationRetrieval>(sql, new
                {
                    StationId = stationId,
                    ClientId = clientId
                });
            }
        }
        public async Task<IEnumerable<StationRetrieval>> GetStationsByClientId(int clientId, int staff_id)
        {
            var (userType, roleName, _, client_Id, station_id) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            string baseSql = @"SELECT s.Id, s.Name, s.Station, s.Location, s.Date_Created, 
                              s.Client_Id, s.Status, c.Name AS ClientName, c.Country AS ClientCountry
                       FROM Station s
                       LEFT JOIN Clients c ON s.Client_Id = c.Id";

            string sql;
            object parameters;

            if (roleName == "Admin")
            {
                sql = baseSql + " WHERE s.Client_Id = @ClientId;";
                parameters = new { ClientId = clientId };
            }
            else if (userType == "Administrator" && roleName == "Manager")
            {
                sql = baseSql + " WHERE s.Client_Id = @ClientId;";
                parameters = new { ClientId = client_Id };
            }
            else if (roleName == "Normal")
            {
                sql = baseSql + " WHERE s.Client_Id = @ClientId AND s.Id = @StationId;";
                parameters = new { ClientId = client_Id, StationId = station_id };
            }
            else
            {
                return Enumerable.Empty<StationRetrieval>();
            }

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<StationRetrieval>(sql, parameters);
            }
        }

        public async Task<string> UpdateStation(Stations station, int staff_id)
        {
            var (_, roleName, _, client_Id, _) =
                await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            if (!roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException( "Unauthorized Request");

            string sql = @"UPDATE Station 
                   SET Name=@Name, Station=@Station, Location=@Location, 
                       Client_Id=@Client_Id, Status=@Status 
                   WHERE Id=@Id"
                           + (roleName.Equals("Manager", StringComparison.OrdinalIgnoreCase)
                              ? " AND Client_Id=@Client_Id" : "") + ";";

            using (var connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    station.Id,
                    station.Name,
                    station.Station,
                    station.Location,
                    station.Status,
                    Client_Id = roleName.Equals("Manager", StringComparison.OrdinalIgnoreCase)
                                ? client_Id : station.Client_Id
                });

                return result > 0 ? "Station updated successfully." : "Station not found or update failed.";
            }
        }

        public async Task<string> DeleteStation(int stationId, int staff_id)
        {
            var (_, roleName, _, _, _) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            if (!roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return "Access denied. Only admins can delete stations.";

            const string sql = "DELETE FROM Station WHERE Id=@StationId;";

            using (var connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { StationId = stationId });
                return result > 0 ? "Station deleted successfully." : "Station not found or could not be deleted.";
            }
        }


        public async Task<IEnumerable<Stations>> GetAllStationsAnalytyics()
        {
            string sql = "SELECT * FROM Station";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Stations>(sql);
            }
        }
    }
}

