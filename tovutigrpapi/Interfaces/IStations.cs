using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IStations
    {
        Task<IEnumerable<Stations>> GetAllStations(int staff_id);
        Task<string> AddStation(Stations station, int staff_id);
        Task<IEnumerable<StationRetrieval>> GetSingleStation(int stationId, int staff_id);
        Task<IEnumerable<StationRetrieval>> GetStationsByClientId(int clientId, int staff_id); 

        Task<string> UpdateStation(Stations station, int staff_id);
        Task<string> DeleteStation(int stationId, int staff_id);
        Task<IEnumerable<Stations>> GetAllStationsAnalytyics();
    }
}

