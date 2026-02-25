namespace AgroSolutions.Dashboard.Models;

public class Alert
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}