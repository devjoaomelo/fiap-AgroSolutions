using AgroSolutions.Dashboard.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace AgroSolutions.Dashboard.Services;

public class FieldService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;

    public FieldService(HttpClient httpClient, AuthStateService authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<List<Field>> GetPropertyFieldsAsync(Guid propertyId)
    {
        SetAuthorizationHeader();

        var response = await _httpClient.GetFromJsonAsync<FieldsResponse>($"api/Fields/property/{propertyId}");
        return response?.Fields ?? new List<Field>();
    }

    public async Task<List<Field>> GetAllUserFieldsAsync(List<Guid> propertyIds)
    {
        var allFields = new List<Field>();

        foreach (var propertyId in propertyIds)
        {
            var fields = await GetPropertyFieldsAsync(propertyId);
            allFields.AddRange(fields);
        }

        return allFields;
    }

    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_authState.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }
    }

    public async Task<bool> CreateFieldAsync(Guid propertyId, string name, string culture, double area)
    {
        SetAuthorizationHeader();

        var request = new
        {
            RuralPropertyId = propertyId,
            Name = name,
            Culture = culture,
            Area = area
        };

        var response = await _httpClient.PostAsJsonAsync("api/Fields", request);
        return response.IsSuccessStatusCode;
    }

    private class FieldsResponse
    {
        public List<Field> Fields { get; set; } = new();
    }
}