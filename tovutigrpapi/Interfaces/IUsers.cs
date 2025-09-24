using tovutigrpapi.Models;

namespace tovutigrpapi.Interfaces
{
    public interface IUsers
    {
        Task<IEnumerable<UsersRetrieval>> GetAllUsers();
        Task<string> AddUser(Users users);
        Task<UsersRetrieval> GetSingleUser(int userId);
        Task<string> UpdateUser(Users user);
        Task<UsersRetrieval> GetUserByEmail(string email);
        Task<string> DeleteUser(int userId);

        Task<IEnumerable<Role>> GetAllRoles();
    }
}
