using AgroSolutions.Dashboard.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace AgroSolutions.Dashboard.Services;

public class PropertyService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;

    public PropertyService(HttpClient httpClient, AuthStateService authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<List<Property>> GetUserPropertiesAsync()
    {
        SetAuthorizationHeader();

        var response = await _httpClient.GetFromJsonAsync<PropertiesResponse>($"api/properties?userId={_authState.UserId}");
        return response?.Properties ?? new List<Property>();
    }

    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_authState.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }
    }

    private class PropertiesResponse
    {
        public List<Property> Properties { get; set; } = new();
    }
}