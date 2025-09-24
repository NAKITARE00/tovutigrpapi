using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Services;

namespace tovutigrpapi.Repositories
{
    public class SparePartRepository : ISparePart
    {
        private readonly DataContext _dataContext;
        private readonly AuthorizationService _authorizationService;
        public SparePartRepository(DataContext dataContext, AuthorizationService authorizationService)
        {
            _dataContext = dataContext;
            _authorizationService = authorizationService;
        }


        public async Task<IEnumerable<SparePart>> GetAllSpareParts(int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            string sql = @"
                SELECT DISTINCT sp.Id, sp.Name, sp.Cost, sp.Status, sp.Station_Id
                FROM Spare_Parts sp
                LEFT JOIN Gadget_SpareParts gsp ON sp.Id = gsp.sparepart_id
                LEFT JOIN Gadgets g ON gsp.gadget_id = g.Id
                LEFT JOIN Station s ON sp.Station_Id = s.Id
                WHERE (g.Id IS NULL OR g.Deleted = 'NotDeleted')";
            

            if (userType == "Administrator" && roleName == "Manager")
            {
                sql += " AND s.Client_Id = @ClientId";
            }
            else if (userType == "Supervisor" && roleName == "Manager")
            {
                sql += " AND s.Client_Id = @ClientId AND s.Id = @StationId";
            }
            else if (roleName == "Normal")
            {
                sql += " AND s.Id = @StationId";
            }

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var spareParts = (await connection.QueryAsync<SparePart>(
                    sql, new { ClientId = clientId, StationId = station_id }
                )).ToList();

                foreach (var part in spareParts)
                {
                    var gadgetNames = await connection.QueryAsync<string>(@"
                SELECT g.Name
                FROM Gadgets g
                JOIN Gadget_SpareParts gsp ON g.Id = gsp.gadget_id
                WHERE gsp.sparepart_id = @SparePartId AND g.Deleted = 'NotDeleted'",
                        new { SparePartId = part.Id });

                    part.LinkedGadgetNames = gadgetNames.ToList();
                }

                return spareParts;
            }
        }

        public async Task<string> AddSparePart(SparePart part, int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                if (userType == "Administrator" && roleName == "Manager")
                {
                    var stationClientId = await connection.ExecuteScalarAsync<int>(
                        "SELECT Client_Id FROM Station WHERE Id = @StationId",
                        new { StationId = part.Station_Id });

                    if (stationClientId != clientId)
                        throw new UnauthorizedAccessException("Administrators with Manager role can only add spare parts under their client.");
                }
                else if (userType == "Supervisor" || roleName == "Normal")
                {
                    if (part.Station_Id != station_id)
                        throw new UnauthorizedAccessException("Supervisors and Normal users can only add spare parts to their assigned station.");
                }
                

                string sql = @"
                INSERT INTO Spare_Parts (Name, Cost, Status, Station_Id)
                VALUES (@Name, @Cost, @Status, @Station_Id);
                ";
                var result = await connection.ExecuteAsync(sql, new
                {
                    part.Name,
                    part.Cost,
                    part.Status,
                    part.Station_Id
                });

                return result > 0 ? "Spare part added successfully." : "Failed to add spare part.";
            }
        }

        public async Task<SparePart> GetSingleSparePart(int sparePartId, int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);

            const string sql = @"SELECT sp.Id, sp.Name, sp.Cost, sp.Status, sp.Station_Id, st.Name AS Station_Name
                         FROM Spare_Parts sp
                         LEFT JOIN Station st ON sp.Station_Id = st.Id
                         WHERE sp.Id = @sparepart_id";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var part = await connection.QueryFirstOrDefaultAsync<SparePart>(sql, new { sparepart_id = sparePartId });

                if (part == null) return null;

                if (userType == "Administrator" && roleName == "Manager")
                {
                    var stationClientId = await connection.ExecuteScalarAsync<int>(
                        "SELECT Client_Id FROM Station WHERE Id = @StationId",
                        new { StationId = part.Station_Id });

                    if (stationClientId != clientId)
                        throw new UnauthorizedAccessException("Managers can only access spare parts under their client.");
                }
                else if (userType == "Supervisor" || roleName == "Normal")
                {
                    if (part.Station_Id != station_id)
                        throw new UnauthorizedAccessException("Supervisors/Normal users can only access spare parts in their station.");
                }

                const string gadgetsSql = @"SELECT g.Name 
                                    FROM Gadgets g
                                    JOIN Gadget_SpareParts gsp ON g.Id = gsp.gadget_id
                                    WHERE gsp.sparepart_id = @sparepart_id";

                var gadgetNames = await connection.QueryAsync<string>(gadgetsSql, new { sparepart_id = part.Id });
                part.LinkedGadgetNames = gadgetNames.ToList();

                return part;
            }
        }

        public async Task<string> UpdateSparePart(SparePart part, int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var existing = await connection.QueryFirstOrDefaultAsync<(int Station_Id, int Client_Id)>(
                    @"SELECT sp.Station_Id, st.Client_Id
              FROM Spare_Parts sp
              JOIN Station st ON sp.Station_Id = st.Id
              WHERE sp.Id = @Id",
                    new { part.Id });

                if (existing.Equals(default((int, int))))
                    return "Spare part not found.";

                if (userType == "Administrator" && roleName == "Manager")
                {
                    if (existing.Client_Id != clientId)
                        throw new UnauthorizedAccessException("Managers can only update spare parts under their client.");
                }
                else if (userType == "Supervisor" || roleName == "Normal")
                {
                    if (existing.Station_Id != station_id)
                        throw new UnauthorizedAccessException("Supervisors/Normal users can only update spare parts in their station.");
                }
               

                const string sql = @"UPDATE Spare_Parts
                             SET Name = @Name,
                                 Cost = @Cost,
                                 Status = @Status,
                                 Station_Id = @Station_Id
                             WHERE Id = @Id";

                var result = await connection.ExecuteAsync(sql, new
                {
                    part.Id,
                    part.Name,
                    part.Cost,
                    part.Status,
                    part.Station_Id
                });

                return result > 0 ? "Spare part updated successfully." : "Failed to update spare part.";
            }
        }


        public async Task<string> DeleteSparePart(int sparePartId, int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);
            using (IDbConnection connection = _dataContext.CreateConnection())
            {   
                if (!roleName.Equals("Admin"))
                    throw new UnauthorizedAccessException("Only Admins Authorized");
                await connection.ExecuteAsync("DELETE FROM Gadget_SpareParts WHERE sparepart_id = @sparepart_id", new { sparepart_id = sparePartId });

                var result = await connection.ExecuteAsync("DELETE FROM Spare_Parts WHERE Id = @sparepart_id", new { sparepart_id = sparePartId });

                return result > 0 ? "Spare part deleted successfully." : "Spare part not found or could not be deleted.";
            }
        }

        public async Task<IEnumerable<SparePart>> GetAllSparePartsAnalytics()
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

    }
}


