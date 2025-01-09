using System.Text;
using System.Text.Json;

namespace UserChecker.ClientTgBot;

public delegate Task Error(string error);



public static class Request
{
    public static Error? OnError = new Error((string error) =>
    {
        Console.WriteLine(error);
        return Task.CompletedTask;
    });
    
    public static async Task<TResponse?> GetConvertedAsync<TResponse>(string url)
    {
        var responseBody = await GetDirectAsync(url);

        if (string.IsNullOrWhiteSpace(responseBody))
        { return default; }
        
        try
        {
            TResponse? obj = JsonSerializer.Deserialize<TResponse>(responseBody);
            return obj;
        }
        catch (JsonException ex)
        {
            await OnError?.Invoke($"Deserialize error: {ex.Message}")!;
            return default;
        }
    }
    
    public static async Task<string?> GetDirectAsync(string url)
    {
        using HttpClient client = new HttpClient();
        try
        {
            HttpResponseMessage response = await client.GetAsync(url);
            return await response.Body();
        }
        catch (Exception ex)
        {
            await OnError?.Invoke($"Request error: {ex.Message}")!;
            return default;
        }
    }
    

    public static async Task<TResponse?> PostConvertedAsync<TResponse, TBody>(string url, TBody body)
    {
        var responseBody = await PostDirectAsync(url, body);

        if (string.IsNullOrWhiteSpace(responseBody))
        { return default; }
        
        try
        {
            TResponse? obj = JsonSerializer.Deserialize<TResponse>(responseBody);
            return obj;
        }
        catch (JsonException ex)
        {
            await OnError?.Invoke($"Deserialize error: {ex.Message}")!;
            return default;
        }
    }
    
    public static async Task<string?> PostDirectAsync<TBody>(string url, TBody body)
    {
        using HttpClient client = new HttpClient();
        
        var jsonContent = JsonSerializer.Serialize(body);
        HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        try
        {
            HttpResponseMessage response = await client.PostAsync(url, content);
            return await response.Body();
        }
        catch (Exception ex)
        {
            await OnError?.Invoke($"Request error: {ex.Message}")!;
            return default;
        }
    }
    
    
    public static async Task<TResponse?> PutConvertedAsync<TResponse, TBody>(string url, TBody body)
    {
        var responseBody = await PutDirectAsync(url, body);

        if (string.IsNullOrWhiteSpace(responseBody))
        { return default; }
        
        try
        {
            TResponse? obj = JsonSerializer.Deserialize<TResponse>(responseBody);
            return obj;
        }
        catch (JsonException ex)
        {
            await OnError?.Invoke($"Deserialize error: {ex.Message}")!;
            return default;
        }
    }
    
    public static async Task<string?> PutDirectAsync<TBody>(string url, TBody body)
    {
        using HttpClient client = new HttpClient();
        
        var jsonContent = JsonSerializer.Serialize(body);
        HttpContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        try
        {
            HttpResponseMessage response = await client.PutAsync(url, content);
            return await response.Body();
        }
        catch (Exception ex)
        {
            await OnError?.Invoke($"Request error: {ex.Message}")!;
            return default;
        }
    }
    
    
    public static async Task<TResponse?> DeleteConvertedAsync<TResponse>(string url)
    {
        var responseBody = await DeleteDirectAsync(url);

        if (string.IsNullOrWhiteSpace(responseBody))
        { return default; }
        
        try
        {
            TResponse? obj = JsonSerializer.Deserialize<TResponse>(responseBody);
            return obj;
        }
        catch (JsonException ex)
        {
            await OnError?.Invoke($"Deserialize error: {ex.Message}")!;
            return default;
        }
    }
    
    public static async Task<string?> DeleteDirectAsync(string url)
    {
        using HttpClient client = new HttpClient();
        
        try
        {
            HttpResponseMessage response = await client.DeleteAsync(url);
            return await response.Body();
        }
        catch (Exception ex)
        {
            await OnError?.Invoke($"Request error: {ex.Message}")!;
            return default;
        }
    }
    
    
    static async Task<string> Body(this HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
}