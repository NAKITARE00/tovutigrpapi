using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IGadgets
    {
        Task<IEnumerable<GadgetRetrieval>> GetAllGadgets(int staff_id); 
        Task<string> AddGadget(Gadgets gadgets, int staff_id);  
        Task<GadgetRetrieval> GetSingleGadget(int gadgetId, int staff_id);
        Task<IEnumerable<GadgetRetrieval>> GetGadgetsByStationId(int stationId, int staff_id);
        Task<string> UpdateGadget(Gadgets gadget, int staff_id);
        Task<string> DeleteGadget(int gadgetId, int staff_id);
        Task<string> AddSparepart(int gadgetId, int sparePartId, int staff_id);
        Task<string> RestoreGadget(int id, int staff_id);
        Task<IEnumerable<GadgetRetrieval>> GetAllGadgetsAnalytics();

    }
}

