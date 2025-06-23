using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IGadgets
    {
        Task<IEnumerable<Gadgets>> GetAllGadgets(); 
        Task<string> AddGadget(Gadgets gadgets);  
        Task<GadgetRetrieval> GetSingleGadget(int gadgetId);
    }
}

