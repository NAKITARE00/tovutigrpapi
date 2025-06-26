using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IUsers
    {
        Task<IEnumerable<Users>> GetAllUsers();
        Task<string> AddUser(Users users);
        Task<Users> GetSingleUser(int userId);
        Task<string> UpdateUser(Users user);
        Task<string> DeleteUser(int userId);
    }
}
