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
                   INSERT INTO Station (Name, Station, Location)
                   VALUES (@Name, @Station, @Location);
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    station.Name,
                    station.Station,
                    station.Location
                });

                if (result > 0)
                {
                    return "Station added successfully.";
                }
                else
                {
                    return "Failed to add station.";
                }
            }
        }
        public async Task <IEnumerable<Stations>> GetAllStations()
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
            Id,
            Name,
            Station,
            Location
        FROM Station
        WHERE Id = @StationId;
        ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<StationRetrieval>(sql, new { StationId = stationId });
            }
        }




    }

}
