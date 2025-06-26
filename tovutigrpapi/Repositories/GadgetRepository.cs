using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Repositories
{
    public class GadgetRepository : IGadgets
    {
        private readonly DataContext _dataContext;

        public GadgetRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<string> AddGadget(Models.Gadgets gadget)
        {
            string sql = @"
                INSERT INTO Gadgets (Name, Status, Station_Id)
                VALUES (@Name, @Status, @Station_Id);
            ";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    gadget.Name,
                    gadget.Status,
                    gadget.Station_Id
                });
                if (result > 0)
                {
                    return "Gadget added successfully.";
                }
                else
                {
                    return "Failed to add gadget.";
                }
            }
        }

        public async Task<IEnumerable<Models.Gadgets>> GetAllGadgets()
        {
            string sql = "SELECT * FROM Gadgets";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<Models.Gadgets>(sql);
            }
        }

        public async Task<GadgetRetrieval> GetSingleGadget(int gadgetId)
        {
            string sql = @"
            SELECT 
                g.Id AS GadgetId,
                g.Name,
                g.Status,
                g.Station_Id,
                s.Name AS StationName,
                s.Location
            FROM Gadgets g
            JOIN Station s ON g.Station_Id = s.Id
            WHERE g.Id = @GadgetId
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<GadgetRetrieval>(sql, new { GadgetId = gadgetId });
            }
        }
        public async Task<IEnumerable<GadgetRetrieval>> GetGadgetsByStationId(int stationId)
        {
            string sql = @"
            SELECT 
                g.Id AS GadgetId,
                g.Name,
                g.Status,
                g.Station_Id,
                s.Name AS StationName,
                s.Location
            FROM Gadgets g
            JOIN Station s ON g.Station_Id = s.Id
            WHERE g.Station_Id = @StationId;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryAsync<GadgetRetrieval>(sql, new { StationId = stationId });
            }
        }



        public async Task<string> UpdateGadget(Gadgets gadget)
        {
            string sql = @"
                UPDATE Gadgets
                SET Name = @Name,
                Status = @Status,
                Station_Id = @Station_Id
                WHERE Id = @Id;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    gadget.Id,
                    gadget.Name,
                    gadget.Status,
                    gadget.Station_Id
                });

                return result > 0 ? "Gadget updated successfully." : "Failed to update gadget or gadget not found.";
            }
        }

        public async Task<string> DeleteGadget(int gadgetId)
        {
            string sql = "DELETE FROM Gadgets WHERE Id = @Id;";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { Id = gadgetId });

                return result > 0 ? "Gadget deleted successfully." : "Gadget not found or could not be deleted.";
            }
        }


    }
}
