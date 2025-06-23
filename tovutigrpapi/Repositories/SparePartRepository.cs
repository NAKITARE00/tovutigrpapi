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
                return await connection.QueryAsync<SparePart>(sql);
            }
        }

        public async Task<string> AddSparePart(SparePart part)
        {
            string sql = @"
                INSERT INTO Spare_Parts (Name, Cost)
                VALUES (@Name, @Cost);
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(sql, new { part.Name, part.Cost });

                return result > 0 ? "Spare part added successfully." : "Failed to add spare part.";
            }
        }

        public async Task<SparePart> GetSingleSparePart(int sparePartId)
        {
            string sql = @"
                SELECT Id, Name, Cost
                FROM Spare_Parts
                WHERE Id = @SparePartId;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<SparePart>(sql, new { SparePartId = sparePartId });
            }
        }
    }
}
