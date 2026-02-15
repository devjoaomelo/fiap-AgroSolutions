namespace AgroSolutions.Alerts.Domain.Entities;

public class Alert
{
    public Guid Id { get; private set; }
    public Guid FieldId { get; private set; }
    public string Type { get; private set; }
    public string Message { get; private set; }
    public string Severity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public bool IsResolved { get; private set; }

    private Alert()
    {
        Type = string.Empty;
        Message = string.Empty;
        Severity = string.Empty;
    }

    public Alert(Guid fieldId, string type, string message, string severity)
    {
        Id = Guid.NewGuid();
        FieldId = fieldId;
        Type = type;
        Message = message;
        Severity = severity;
        CreatedAt = DateTime.UtcNow;
        IsResolved = false;
    }

    public void Resolve()
    {
        IsResolved = true;
        ResolvedAt = DateTime.UtcNow;
    }
}