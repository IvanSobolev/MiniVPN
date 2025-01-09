using UserChecker.Server.Model;
using UserChecker.Server.Service.Interfaces;

namespace UserChecker.Server.Service.Implimentations;

public class UserRepository(DataContext dataContext) : IUserRepository
{
    private readonly DataContext _context = dataContext;
    
    public IEnumerable<User> GetAllUsers()
    {
        return _context.Users;
    }

    public void AddUser(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void UpdateUser(User user, User old)
    {
        _context.Entry(old).CurrentValues.SetValues(user);
        _context.SaveChanges();
    }

    public void DeleteUser(User user)
    {
        _context.Users.Remove(user);
        _context.SaveChanges();
    }
}