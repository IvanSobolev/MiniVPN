using UserChecker.Server.Model;

namespace UserChecker.Server.Service.Interfaces;

public interface IUserManager
{
    User? GetUserById(long id);
    User? GetUserByTgId(long tgId);
    IEnumerable<User> GetAllUserSortedByTime();
    bool AddDaysToUserByTgId(long tgId, int day);
    bool UpdateActiveCodeByTgId(long tgId, string activeCode);
    
    IEnumerable<User> GetAllUsers();
    void AddUser(UserRequestDTO userDTO);
    bool UpdateUser(User user);
    bool UpdateUserByTgId(User user);
    bool DeleteUser(long id);
    bool DeleteUserByTgId(long id);
}