using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface ISparePart
    {
        Task<IEnumerable<SparePart>> GetAllSpareParts(int staff_id);
        Task<string> AddSparePart(SparePart part, int staff_id);
        Task<SparePart> GetSingleSparePart(int sparePartId, int staff_id);
        Task<string> UpdateSparePart(SparePart part, int staff_id);
        Task<string> DeleteSparePart(int sparePartId, int staff_id);
        Task<IEnumerable<SparePart>> GetAllSparePartsAnalytics();

    }
}

