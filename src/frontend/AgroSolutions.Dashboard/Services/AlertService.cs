using AgroSolutions.Dashboard.Models;
using System.Net.Http.Headers;
using System.Text.Json;

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

    private void SetAuthorizationHeader()
    {
        if (!string.IsNullOrEmpty(_authState.Token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authState.Token);
        }
    }

    public async Task<List<Alert>> GetFieldAlertsAsync(Guid fieldId)
    {
        try
        {
            SetAuthorizationHeader();

            var url = $"api/Alerts?fieldId={fieldId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return new List<Alert>();
            }

            var content = await response.Content.ReadAsStringAsync();

            if (content.Contains("\"alerts\""))
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var wrapper = JsonSerializer.Deserialize<AlertsWrapper>(content, options);
                return wrapper?.Alerts ?? new List<Alert>();
            }

            // Se não tiver wrapper, tentar lista direta
            var alerts = JsonSerializer.Deserialize<List<Alert>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return alerts ?? new List<Alert>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ALERT SERVICE ERROR] {ex.Message}");
            Console.WriteLine($"[ALERT SERVICE ERROR] Stack: {ex.StackTrace}");
            return new List<Alert>();
        }
    }

    public async Task<bool> ResolveAlertAsync(Guid alertId)
    {
        SetAuthorizationHeader();
        var response = await _httpClient.PatchAsync($"api/Alerts/{alertId}/resolve", null);
        return response.IsSuccessStatusCode;
    }
}

public class AlertsWrapper
{
    public List<Alert> Alerts { get; set; } = new();
}