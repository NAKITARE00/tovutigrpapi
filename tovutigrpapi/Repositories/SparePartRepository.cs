using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;

namespace tovutigrpapi.Repositories
{
    public class SparePartRepository : ISparePart
    {
        private readonly DataContext _dataContext;

        public SparePartRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IEnumerable<SparePart>> GetAllSpareParts()
        {
            string sql = "SELECT * FROM Spare_Parts";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var spareParts = await connection.QueryAsync<SparePart>(sql);

                foreach (var part in spareParts)
                {
                    var gadgetNames = await connection.QueryAsync<string>(@"
                        SELECT g.Name 
                        FROM Gadgets g
                        JOIN Gadget_SpareParts gsp ON g.Id = gsp.gadget_id
                        WHERE gsp.sparepart_id = @sparepart_id", new { sparepart_id = part.Id });

                    part.LinkedGadgetNames = gadgetNames.ToList();
                }
                return spareParts;
            }
        }
        public async Task<string> AddSparePart(SparePart part)
        {
            string sql = @"
                INSERT INTO Spare_Parts (Name, Cost, Status, Station_Id)
                VALUES (@Name, @Cost, @Status, @Station_Id);
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { part.Name, part.Cost, part.Status, part.Station_Id });

                return result > 0 ? "Spare part added successfully." : "Failed to add spare part.";
            }
        }

        public async Task<SparePart> GetSingleSparePart(int sparePartId)
        {
            string sql = @"
                SELECT 
                    sp.Id, 
                    sp.Name, 
                    sp.Cost, 
                    sp.Status, 
                    sp.Station_Id, 
                    st.Name as Station_Name
                FROM Spare_Parts Sp
                LEFT JOIN Station st ON sp.Station_Id = st.Id
                WHERE sp.Id = @sparepart_id;
                ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var part = await connection.QueryFirstOrDefaultAsync<SparePart>(sql, new { sparepart_id = sparePartId });

                if (part != null)
                {
                    var gadgetNames = await connection.QueryAsync<string>(@"
                        SELECT g.Name 
                        FROM Gadgets g
                        JOIN Gadget_SpareParts gsp ON g.Id = gsp.gadget_id
                        WHERE gsp.sparepart_id = @sparepart_id", new { sparepart_id = part.Id });

                    part.LinkedGadgetNames = gadgetNames.ToList();
                }

                return part;
            }
        }

        public async Task<string> UpdateSparePart(SparePart part)
        {
            string sql = @"
                UPDATE Spare_Parts
                SET Name = @Name,
                    Cost = @Cost,
                    Status = @Status,
                    Station_Id = @Station_Id
                WHERE Id = @Id;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new
                {
                    part.Id,
                    part.Name,
                    part.Cost,
                    part.Status,
                    part.Station_Id
                });

                return result > 0 ? "Spare part updated successfully." : "Failed to update spare part or part not found.";
            }
        }

        public async Task<string> DeleteSparePart(int sparePartId)
        {
            using (IDbConnection connection = _dataContext.CreateConnection())
            {   
                await connection.ExecuteAsync("DELETE FROM Gadget_SpareParts WHERE sparepart_id = @sparepart_id", new { sparepart_id = sparePartId });

                var result = await connection.ExecuteAsync("DELETE FROM Spare_Parts WHERE Id = @sparepart_id", new { sparepart_id = sparePartId });

                return result > 0 ? "Spare part deleted successfully." : "Spare part not found or could not be deleted.";
            }
        }
    }
}


