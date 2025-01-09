using UserChecker.Server.Model;

namespace UserChecker.ClientTgBot;

public interface IDataRepository
{
    public Task<IEnumerable<UserDB>?> GetAll();
    public Task<IEnumerable<UserDB>?> GetAllSorted();
    public Task<UserDB?> GetById(long id);
    public Task<UserDB?> GetByTgId(long tgId);
    
    public Task AddUser(UserRequestDTO user);
    
    public Task UpdateUserById(UserDB user);
    public Task UpdateUserByTgId(UserDB user);
    public Task AddDaysToUser(long tgId, int dayCount);
    public Task UpdateCodeToUser(long tgId, string newCode);
    
    public Task DeleteUserById(long id);
    public Task DeleteUserByTgId(long tgId);
    
}