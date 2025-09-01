using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IUsers
    {
        Task<IEnumerable<Users>> GetAllUsers();
        Task<string> AddUser(Users users);
        Task<Users> GetSingleUser(int userId);
        Task<string> UpdateUser(Users user);
        Task<Users> GetUserByEmail(string email);
        Task<string> DeleteUser(int userId);

        Task<IEnumerable<Role>> GetAllRoles();
    }
}
