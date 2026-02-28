using AgroSolutions.Dashboard.Models;
using System.Net.Http.Json;

namespace AgroSolutions.Dashboard.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> RegisterAsync(string name, string email, string password)
    {
        var request = new
        {
            Name = name,
            Email = email,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        return response.IsSuccessStatusCode;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

                return loginResponse;
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in LoginAsync: {ex.Message}");
            throw;
        }
    }
}