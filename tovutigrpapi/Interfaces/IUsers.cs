using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IUsers
    {
        Task<IEnumerable<Users>> GetAllUsers();
        Task<String> AddUser(Users users);
    }
}
