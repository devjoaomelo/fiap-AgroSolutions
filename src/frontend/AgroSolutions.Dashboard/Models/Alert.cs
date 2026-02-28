using System.Text.Json.Serialization;

namespace AgroSolutions.Dashboard.Models;

public class Alert
{
    [JsonPropertyName("alertId")]
    public Guid Id { get; set; }

    [JsonPropertyName("fieldId")]
    public Guid FieldId { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("isResolved")]
    public bool IsResolved { get; set; }
}
public class AlertsListResponse
{
    public List<Alert> Alerts { get; set; } = new();
}