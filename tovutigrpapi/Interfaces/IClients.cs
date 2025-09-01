using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IClients
    {
        Task<IEnumerable<Client>> GetAllClients(int staff_id);
        Task<Client> GetSingleClient(int id, int staff_id);
        Task<string> AddClient(Client client, int staff_id);
        Task<string> UpdateClient(Client client, int staff_id);
        Task<string> DeleteClient(int id, int staff_id);
        Task<IEnumerable<Client>> GetAllClientsAnalytics();
    }
}
