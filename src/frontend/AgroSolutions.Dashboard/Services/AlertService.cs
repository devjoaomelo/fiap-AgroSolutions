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

        var alerts = await _httpClient.GetFromJsonAsync<List<Alert>>(url);
        return alerts ?? new List<Alert>();
    }
}