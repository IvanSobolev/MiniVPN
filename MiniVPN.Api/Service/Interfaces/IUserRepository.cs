using UserChecker.Server.Model;

namespace UserChecker.Server.Service.Interfaces;

public interface IUserRepository
{
    IEnumerable<User> GetAllUsers();
    void AddUser(User user);
    void UpdateUser(User user, User old);
    void DeleteUser(User user);
}