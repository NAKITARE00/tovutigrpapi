using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IStations
    {
        Task<IEnumerable<Stations>> GetAllStations();
        Task<String> AddStation(Stations station);
        Task<IEnumerable<StationRetrieval>> GetSingleStation(int stationId);


    }
}

