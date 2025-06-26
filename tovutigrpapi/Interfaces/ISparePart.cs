using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface ISparePart
    {
        Task<IEnumerable<SparePart>> GetAllSpareParts();
        Task<string> AddSparePart(SparePart part);
        Task<SparePart> GetSingleSparePart(int sparePartId);
        Task<string> UpdateSparePart(SparePart part);
        Task<string> DeleteSparePart(int sparePartId);
    }
}

