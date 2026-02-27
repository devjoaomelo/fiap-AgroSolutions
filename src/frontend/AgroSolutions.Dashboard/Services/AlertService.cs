using AgroSolutions.Dashboard.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AgroSolutions.Dashboard.Services;

public class AlertService
{
    private readonly HttpClient _httpClient;
    private readonly AuthStateService _authState;

    public AlertService(HttpClient httpClient, AuthStateService authState)
    {
        _httpClient = httpClient;
        _authState = authState;
    }

    public async Task<List<Alert>> GetAlertsAsync(Guid? fieldId = null)
    {
        var url = fieldId.HasValue
            ? $"api/alerts?fieldId={fieldId}"
            : "api/alerts";

        var response = await _httpClient.GetFromJsonAsync<AlertsResponse>(url);
        return response?.Alerts ?? new List<Alert>();
    }

    public async Task<bool> ResolveAlertAsync(Guid alertId)
    {
        SetAuthorizationHeader();

        var response = await _httpClient.PutAsync($"api/alerts/{alertId}/resolve", null);
        return response.IsSuccessStatusCode;
    }

    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_authState.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }
    }

    // Classe interna para mapear a resposta da API
    private class AlertsResponse
    {
        public List<Alert> Alerts { get; set; } = new();
    }
}