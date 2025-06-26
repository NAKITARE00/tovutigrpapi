using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IGadgets
    {
        Task<IEnumerable<Gadgets>> GetAllGadgets(); 
        Task<string> AddGadget(Gadgets gadgets);  
        Task<GadgetRetrieval> GetSingleGadget(int gadgetId);
        Task<IEnumerable<GadgetRetrieval>> GetGadgetsByStationId(int stationId);
        Task<string> UpdateGadget(Gadgets gadget);
        Task<string> DeleteGadget(int gadgetId);


    }
}

