using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IClients
    {
        Task<IEnumerable<Client>> GetAllClients();
        Task<Client> GetSingleClient(int id);
        Task<string> AddClient(Client client);
        Task<string> UpdateClient(Client client);
        Task<string> DeleteClient(int id);
    }
}
