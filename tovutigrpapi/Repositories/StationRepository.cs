using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Repositories
{
    public class StationRepository : IStations
    {
        private readonly DataContext _dataContext;

        public StationRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<string> AddStation(Stations station)
        {
            string sql = @"
                INSERT INTO Station (Name, Station, Location, Client_Id)
                VALUES (@Name, @Station, @Location, @Client_Id);
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    station.Name,
                    station.Station,
                    station.Location,
                    station.Client_Id
                });

                return result > 0 ? "Station added successfully." : "Failed to add station.";
            }
        }

        public async Task<IEnumerable<Stations>> GetAllStations()
        {
            string sql = "SELECT * FROM Station";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Stations>(sql);
            }
        }

        public async Task<IEnumerable<StationRetrieval>> GetSingleStation(int stationId)
        {
            string sql = @"
        SELECT 
            s.Id,
            s.Name,
            s.Station,
            s.Location,
            s.Client_Id,
            c.Name AS ClientName,
            c.Country AS ClientCountry
        FROM Station s
        LEFT JOIN Clients c ON s.Client_Id = c.Id
        WHERE s.Id = @StationId;
    ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<StationRetrieval>(sql, new { StationId = stationId });
            }
        }

        public async Task<IEnumerable<StationRetrieval>> GetStationsByClientId(int clientId)
        {
            string sql = @"
        SELECT 
            s.Id,
            s.Name,
            s.Station,
            s.Location,
            s.Client_Id,
            c.Name AS ClientName,
            c.Country AS ClientCountry
        FROM Station s
        LEFT JOIN Clients c ON s.Client_Id = c.Id
        WHERE s.Client_Id = @ClientId;
    ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<StationRetrieval>(sql, new { ClientId = clientId });
            }
        }

        public async Task<string> UpdateStation(Stations station)
        {
            string sql = @"
                UPDATE Station
                SET Name = @Name,
                    Station = @Station,
                    Location = @Location,
                    Client_Id = @Client_Id
                WHERE Id = @Id;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    station.Id,
                    station.Name,
                    station.Station,
                    station.Location,
                    station.Client_Id
                });

                return result > 0 ? "Station updated successfully." : "Station not found or update failed.";
            }
        }

        public async Task<string> DeleteStation(int stationId)
        {
            string sql = "DELETE FROM Station WHERE Id = @StationId;";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { StationId = stationId });

                return result > 0 ? "Station deleted successfully." : "Station not found or could not be deleted.";
            }
        }
    }
}

