using System.Text.Json.Serialization;

namespace AgroSolutions.Dashboard.Models;

public class Property
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("userId")]
    public Guid UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("totalArea")]
    public double TotalArea { get; set; }
}