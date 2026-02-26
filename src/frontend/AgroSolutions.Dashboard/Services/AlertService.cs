using AgroSolutions.Dashboard.Models;
using System.Net.Http.Json;

namespace AgroSolutions.Dashboard.Services;

public class AlertService
{
    private readonly HttpClient _httpClient;

    public AlertService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Alert>> GetAlertsAsync(Guid? fieldId = null)
    {
        var url = fieldId.HasValue
            ? $"api/alerts?fieldId={fieldId}"
            : "api/alerts";

        // Criar modelo intermediario para deserializar a resposta
        var response = await _httpClient.GetFromJsonAsync<AlertsResponse>(url);
        return response?.Alerts ?? new List<Alert>();
    }

    // Classe interna para mapear a resposta da API
    private class AlertsResponse
    {
        public List<Alert> Alerts { get; set; } = new();
    }
}