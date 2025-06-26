using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IStations
    {
        Task<IEnumerable<Stations>> GetAllStations();
        Task<string> AddStation(Stations station);
        Task<IEnumerable<StationRetrieval>> GetSingleStation(int stationId);
        Task<IEnumerable<StationRetrieval>> GetStationsByClientId(int clientId); // Add this if needed

        Task<string> UpdateStation(Stations station);
        Task<string> DeleteStation(int stationId);
    }
}

