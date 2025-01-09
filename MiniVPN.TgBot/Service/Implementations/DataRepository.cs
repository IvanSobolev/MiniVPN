using UserChecker.Server.Model;

namespace UserChecker.ClientTgBot;

public class DataRepository(string baseUrl = "http://localhost:5010/", string apiPath = "User/") : IDataRepository
{
    private readonly string _baseUrl = baseUrl;
    private readonly string _apiPath = apiPath;

    public async Task<IEnumerable<UserDB>?> GetAll()
    {
        return await Request.GetConvertedAsync<IEnumerable<UserDB>>(_baseUrl + _apiPath + "admin/get/all");
    }

    public async Task<IEnumerable<UserDB>?> GetAllSorted()
    {
        return await Request.GetConvertedAsync<IEnumerable<UserDB>>(_baseUrl + _apiPath + "admin/getsort");
    }

    public async Task<UserDB?> GetById(long id)
    {
        return await Request.GetConvertedAsync<UserDB>(_baseUrl + _apiPath + $"admin/get/{id}");
    }
    
    public async Task<UserDB?> GetByTgId(long tgId)
    {
        return await Request.GetConvertedAsync<UserDB>(_baseUrl + _apiPath + $"get/{tgId}");
    }

    public async Task AddUser(UserRequestDTO user)
    {
        await Request.PostDirectAsync(_baseUrl + _apiPath + $"admin/add", user);
    }

    public async Task UpdateUserById(UserDB user)
    {
        await Request.PutDirectAsync(_baseUrl + _apiPath + $"admin/update", user);
    }

    public async Task UpdateUserByTgId(UserDB user)
    {
        await Request.PutDirectAsync(_baseUrl + _apiPath + $"admin/updatetg", user);
    }

    public async Task AddDaysToUser(long tgId, int dayCount)
    {
        await Request.PutDirectAsync(_baseUrl + _apiPath + $"admin/adddays/{tgId}/{dayCount}", 0);
    }

    public async Task UpdateCodeToUser(long tgId, string newCode)
    {
        await Request.PutDirectAsync(_baseUrl + _apiPath + $"admin/updatecode/{tgId}/{newCode}", 0);
    }

    public async Task DeleteUserById(long id)
    {
        await Request.DeleteDirectAsync(_baseUrl + _apiPath + $"admin/delete/{id}");
    }

    public async Task DeleteUserByTgId(long tgId)
    {
        await Request.DeleteDirectAsync(_baseUrl + $"api/User/admin/deletetg/{tgId}");
    }
}