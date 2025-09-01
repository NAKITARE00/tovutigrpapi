using Dapper;
using System.Data;
using tovutigrpapi.DataAccess;
using tovutigrpapi.Interfaces;
using tovutigrpapi.Models;
using tovutigrpapi.Services;

namespace tovutigrpapi.Repositories
{
    public class GadgetRepository : IGadgets
    {
        private readonly DataContext _dataContext;
        private readonly AuthorizationService _authorizationService;
        public GadgetRepository(DataContext dataContext, AuthorizationService authorizationService)
        {
            _dataContext = dataContext;
            _authorizationService = authorizationService;
        }

        public async Task<string> AddGadget(Models.Gadgets gadget, int staff_id)
        {
            var (_, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            using (var connection = _dataContext.CreateConnection())
            {
                if (roleName.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                {
                    var stationCheck = await connection.ExecuteScalarAsync<int>(
                        "SELECT COUNT(1) FROM Station WHERE Id = @StationId AND Client_Id = @ClientId",
                        new { StationId = gadget.Station_Id, ClientId = clientId });

                    if (stationCheck == 0 || gadget.Station_Id != station_id)
                    {
                        throw new UnauthorizedAccessException("Manager not authorized to add gadget to this station.");
                    }
                }
                else if (!roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Only Admins and authorized Supervisors can add gadgets.");
                }
                string insertGadgetSql = @"
            INSERT INTO Gadgets (Name, Status, Station_Id, Serial_No, IMEI1, IMEI2)
            VALUES (@Name, @Status, @Station_Id, @Serial_No, @IMEI1, @IMEI2);
            SELECT CAST(SCOPE_IDENTITY() as int);";

                var gadgetId = await connection.QuerySingleAsync<int>(insertGadgetSql, new
                {
                    gadget.Name,
                    gadget.Status,
                    gadget.Station_Id,
                    gadget.Serial_No,
                    gadget.Imei1,
                    gadget.Imei2
                });
                if (gadget.SparePartIds != null && gadget.SparePartIds.Any())
                {
                    foreach (var sparePartId in gadget.SparePartIds)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO Gadget_SpareParts (gadget_id, sparepart_id) VALUES (@GadgetId, @SparePartId)",
                            new { GadgetId = gadgetId, SparePartId = sparePartId });
                    }
                }
                return "Gadget added successfully.";
            }
        }
        public async Task<IEnumerable<GadgetRetrieval>> GetAllGadgets(int staff_id)
        {
            var (userType, roleName, _, clientId, station_id) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            string sql = "SELECT g.Id AS GadgetId, g.Name, g.Status, g.Station_Id, g.Serial_No, g.IMEI1, g.IMEI2, " +
                         "s.Name AS StationName, s.Location " +
                         "FROM Gadgets g JOIN Station s ON g.Station_Id = s.Id " +
                         "WHERE g.Deleted = 'NotDeleted'";

            if (!roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                if (userType == "Administrator" && roleName == "Manager")
                    sql += " AND s.Client_Id = @ClientId";
                else if (userType == "Supervisor" && roleName == "Manager")
                    sql += " AND s.Client_Id = @ClientId AND s.Id = @StationId";
                else if (roleName == "Normal")
                    sql += " AND s.Id = @StationId";
            }
            using (var connection = _dataContext.CreateConnection())
            {
                var gadgets = (await connection.QueryAsync<GadgetRetrieval>(sql, new { ClientId = clientId, StationId = station_id })).ToList();

                foreach (var gadget in gadgets)
                {
                    var sparePartNames = await connection.QueryAsync<string>(
                        "SELECT sp.Name FROM Spare_Parts sp " +
                        "JOIN Gadget_SpareParts gsp ON sp.Id = gsp.sparepart_id " +
                        "WHERE gsp.gadget_id = @GadgetId",
                        new { GadgetId = gadget.GadgetId });

                    gadget.SparePartNames = sparePartNames.ToList();
                }

                return gadgets;
            }
        }
        public async Task<GadgetRetrieval> GetSingleGadget(int gadgetId, int staff_id)
        {
            var (userType, roleName, client_id, _, station_id) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");
            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");
            string clientCheckSql = @"SELECT Client_Id, Station_Id 
                              FROM Gadgets 
                              WHERE Id = @GadgetId;";
            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var gadgetData = await connection.QueryFirstOrDefaultAsync<(int Client_Id, int Station_Id)>(
                    clientCheckSql, new { GadgetId = gadgetId });

                if (gadgetData.Equals(default((int, int))))
                    return null;
                if (userType == "Administrator" && roleName == "Manager" && gadgetData.Client_Id != client_id)
                    throw new UnauthorizedAccessException("Managers can only access gadgets under their client.");
                if (userType == "Operator" && roleName == "Normal" && gadgetData.Station_Id != station_id)
                    throw new UnauthorizedAccessException("Operators can only access gadgets belonging to their station.");
                if (userType == "Supervisor" && gadgetData.Station_Id != station_id)
                    throw new UnauthorizedAccessException("Station Managers can only access gadgets belonging to their station.");
                string sql = @"
            SELECT 
                g.Id,
                g.Name,
                g.Description,
                g.SerialNumber,
                g.Category,
                g.Station_Id,
                st.Name AS Station_Name
            FROM Gadgets g
            LEFT JOIN Station st ON g.Station_Id = st.Id
            WHERE g.Id = @GadgetId;
            ";
                return await connection.QueryFirstOrDefaultAsync<GadgetRetrieval>(sql, new { GadgetId = gadgetId });
            }
        }
        public async Task<string> AddSparepart(int gadgetId, int sparePartId, int staff_id)
        {
            var (userType, roleName, _, clientId, staffId) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var gadgetClientId = await connection.QueryFirstOrDefaultAsync<int>(
                    @"SELECT s.Client_Id 
                    FROM Gadgets g
                    INNER JOIN Stations s ON g.Station_Id = s.Id
                     WHERE g.Id = @GadgetId;",
                    new { GadgetId = gadgetId });

                if (gadgetClientId == 0)
                    throw new KeyNotFoundException("Gadget not found.");
                if (userType == "Supervisor" || roleName == "Normal")
                    throw new UnauthorizedAccessException("Cannot add spare parts.");
                if (userType == "Administrator" && roleName == "Manager" && gadgetClientId != clientId)
                    throw new UnauthorizedAccessException("Unauthorized Client");
               var result = await connection.ExecuteAsync(
                    @"INSERT INTO Gadget_SpareParts (gadget_id, sparepart_id)
                    VALUES (@GadgetId, @SparePartId);",
                    new { GadgetId = gadgetId, SparePartId = sparePartId });

                return result > 0
                    ? "Spare part added to gadget successfully."
                    : "Failed to add spare part to gadget.";
            }
        }
        public async Task<IEnumerable<GadgetRetrieval>> GetGadgetsByStationId(int stationId, int staff_id)
        {
            var (userType, roleName, client_id, _, station_id) = await _authorizationService.GetUserRole(staff_id);
            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");
            if (roleName != "Admin" && roleName != "Manager" && roleName != "Normal")
                throw new UnauthorizedAccessException("User role not recognized.");

            string sql = @"
            SELECT g.Id AS GadgetId, g.Name, g.Status, g.Station_Id,
               g.Serial_No, g.IMEI1, g.IMEI2,
               s.Name AS StationName, s.Location
            FROM Gadgets g
            JOIN Station s ON g.Station_Id = s.Id
            WHERE g.Station_Id = @StationId;
            ";

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var gadgets = (await connection.QueryAsync<GadgetRetrieval>(sql, new { StationId = stationId })).ToList();

                if (userType == "Administrator" && roleName == "Manager")
                {
                    var stationCheck = await connection.QueryFirstOrDefaultAsync<int?>(
                        "SELECT Client_Id FROM Station WHERE Id = @StationId",
                        new { StationId = stationId });

                    if (stationCheck == null || stationCheck != client_id)
                        throw new UnauthorizedAccessException("Managers can only access gadgets under their client.");
                }
                else if (userType == "Supervisor" || roleName == "Normal")
                {                    if (stationId != station_id)
                        throw new UnauthorizedAccessException("Operators can only access gadgets belonging to their station.");
                }

                if (!gadgets.Any())
                    throw new UnauthorizedAccessException("You are not authorized to access gadgets for this station.");

                foreach (var gadget in gadgets)
                {
                    var sparePartNames = await connection.QueryAsync<string>(@"
                SELECT sp.Name 
                FROM Spare_Parts sp
                JOIN Gadget_SpareParts gsp ON sp.Id = gsp.sparepart_id
                WHERE gsp.gadget_id = @GadgetId",
                        new { GadgetId = gadget.GadgetId });

                    gadget.SparePartNames = sparePartNames.ToList();
                }

                return gadgets;
            }
        }

        public async Task<string> UpdateGadget(Gadgets gadget, int staff_id)
        {
            var (userType, roleName, _, clientId, stationId) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {

                string clientSql = "SELECT s.Client_Id FROM Gadgets g JOIN Station s ON g.Station_Id = s.Id WHERE g.Id = @Id;";
                var gadgetClientId = await connection.QueryFirstOrDefaultAsync<int?>(clientSql, new { gadget.Id });

                if (gadgetClientId == null)
                    throw new KeyNotFoundException("Gadget not found.");

                if (roleName == "Normal")
                    throw new UnauthorizedAccessException("Normal users are not allowed to update gadgets.");
                if (userType == "Administrator" && roleName == "Manager" && gadgetClientId != clientId)
                    throw new UnauthorizedAccessException("Manager cannot update gadgets outside their client scope.");
                if (userType == "Supervisor" && roleName == "Manager" && gadget.Station_Id != stationId)
                    throw new UnauthorizedAccessException("Station Managers cannot update gadgets outside station scope.");

                string updateSql = "UPDATE Gadgets SET Name=@Name, Status=@Status, Station_Id=@Station_Id, Serial_No=@Serial_No, IMEI1=@IMEI1, IMEI2=@IMEI2 WHERE Id=@Id;";
                await connection.ExecuteAsync(updateSql, new
                {
                    gadget.Id,
                    gadget.Name,
                    gadget.Status,
                    gadget.Station_Id,
                    gadget.Serial_No,
                    gadget.Imei1,
                    gadget.Imei2
                });

                await connection.ExecuteAsync("DELETE FROM Gadget_SpareParts WHERE gadget_id=@gadget_id", new { gadget_id = gadget.Id });

                if (gadget.SparePartIds != null)
                {
                    foreach (var sparePartId in gadget.SparePartIds)
                    {
                        await connection.ExecuteAsync(
                            "INSERT INTO Gadget_SpareParts (gadget_id, sparepart_id) VALUES (@gadget_id, @sparepart_id)",
                            new { gadget_id = gadget.Id, sparepart_id = sparePartId });
                    }
                }

                return "Gadget updated successfully.";
            }
        }


        public async Task<string> DeleteGadget(int gadgetId, int staff_id)
        {
            var (_, roleName, _, _, _) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin")
                throw new UnauthorizedAccessException("Only admins can delete gadgets.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(
                    "UPDATE Gadgets SET Deleted = 'isDeleted' WHERE Id = @Id;",
                    new { Id = gadgetId });

                return result > 0
                    ? "Gadget soft deleted successfully."
                    : "Gadget not found or already deleted.";
            }
        }


        public async Task<string> RestoreGadget(int gadgetId, int staff_id)
        {
            var (_, roleName, _, _, _) = await _authorizationService.GetUserRole(staff_id);

            if (string.IsNullOrEmpty(roleName))
                throw new UnauthorizedAccessException("User has no assigned role.");

            if (roleName != "Admin")
                throw new UnauthorizedAccessException("Only admins can restore gadgets.");

            using (IDbConnection connection = _dataContext.CreateConnection())
            {
                var result = await connection.ExecuteAsync(
                    "UPDATE Gadgets SET Deleted = 'notDeleted' WHERE Id = @Id;",
                    new { Id = gadgetId });

                return result > 0
                    ? "Gadget restored successfully."
                    : "Gadget not found or already active.";
            }
        }


        public async Task<IEnumerable<GadgetRetrieval>> GetAllGadgetsAnalytics()
        {
            string sql = @"SELECT * FROM Gadgets WHERE Deleted = 'NotDeleted'";
            using (var connection = _dataContext.CreateConnection())
            {
                var gadgets = (await connection.QueryAsync<GadgetRetrieval>(sql)).ToList();
                foreach (var gadget in gadgets)
                {
                    var sparePartNames = await connection.QueryAsync<string>(
                        @"SELECT sp.Name 
                  FROM Spare_Parts sp
                  JOIN Gadget_SpareParts gsp ON sp.Id = gsp.sparepart_id
                  WHERE gsp.gadget_id = @GadgetId",
                        new { GadgetId = gadget.GadgetId });
                    gadget.SparePartNames = sparePartNames.ToList();
                }
                return gadgets;
            }

        }
    }
}
