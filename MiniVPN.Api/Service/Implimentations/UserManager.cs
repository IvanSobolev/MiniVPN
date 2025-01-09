using UserChecker.Server.Model;
using UserChecker.Server.Service.Interfaces;

namespace UserChecker.Server.Service.Implimentations;

public class UserManager(IUserRepository userRepository) : IUserManager
{
    private readonly IUserRepository _userRepository = userRepository;
    
    public IEnumerable<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }

    public void AddUser(UserRequestDTO userDTO)
    {
        var user = new User(userDTO);
        _userRepository.AddUser(user);
    }

    public bool UpdateUser(User user)
    {
        var existing = _userRepository.GetAllUsers().FirstOrDefault(u => u.Id == user.Id);

        if (existing == default)
        { return false; }
        
        _userRepository.UpdateUser(user, existing);
        return true;
    }

    public bool UpdateUserByTgId(User user)
    {
        var existing = _userRepository.GetAllUsers().FirstOrDefault(u => u.TgId == user.TgId);

        if (existing == default)
        { return false; }
        
        _userRepository.UpdateUser(user, existing);
        return true;
    }

    public bool DeleteUser(long id)
    {
        var existing = _userRepository.GetAllUsers().FirstOrDefault(u => u.Id == id);

        if (existing == default)
        {
            return false;
        }
        _userRepository.DeleteUser(existing);
        return true;
    }

    public bool DeleteUserByTgId(long id)
    {
        var existing = _userRepository.GetAllUsers().FirstOrDefault(u => u.TgId == id);

        if (existing == default)
        {
            return false;
        }
        _userRepository.DeleteUser(existing);
        return true;
    }

    public User? GetUserById(long id)
    {
        return _userRepository.GetAllUsers().FirstOrDefault(u => u.Id == id);
    }

    public User? GetUserByTgId(long tgId)
    {
        return _userRepository.GetAllUsers().FirstOrDefault(u => u.TgId == tgId);
    }

    public IEnumerable<User> GetAllUserSortedByTime()
    {
        return _userRepository.GetAllUsers().OrderBy(u => u.PaidUntil);
    }

    public bool AddDaysToUserByTgId(long tgId, int dayCount)
    {
        var existing = _userRepository.GetAllUsers().FirstOrDefault(u => u.TgId == tgId);

        if (existing == default)
        { return false; }
        var newEx = existing;
        newEx.PaidUntil = existing.PaidUntil.AddDays(dayCount);
        _userRepository.UpdateUser(newEx,existing);
        return true;
    }

    public bool UpdateActiveCodeByTgId(long tgId, string activeCode)
    {
        var existing = _userRepository.GetAllUsers().FirstOrDefault(u => u.TgId == tgId);

        if (existing == default)
        { return false; }
        var newEx = existing;
        newEx.ActualVpnCode = activeCode;
        _userRepository.UpdateUser(newEx,existing);
        return true;
    }
}