using System.Text.Json.Serialization;

namespace AgroSolutions.Dashboard.Models;

public class Field
{
    [JsonPropertyName("fieldId")]
    public Guid Id { get; set; }

    [JsonPropertyName("ruralPropertyId")]
    public Guid RuralPropertyId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("culture")]
    public string Culture { get; set; } = string.Empty;

    [JsonPropertyName("area")]
    public double Area { get; set; }
}