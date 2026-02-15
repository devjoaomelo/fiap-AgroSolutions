namespace AgroSolutions.Ingestion.Domain.Entities;

public class SensorData
{
    public Guid Id { get; private set; }
    public Guid FieldId { get; private set; }
    public double SoilMoisture { get; private set; }
    public double Temperature { get; private set; }
    public double Precipitation { get; private set; }
    public DateTime Timestamp { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private SensorData() { }

    public SensorData(Guid fieldId, double soilMoisture, double temperature, double precipitation, DateTime timestamp)
    {
        Id = Guid.NewGuid();
        FieldId = fieldId;
        SoilMoisture = soilMoisture;
        Temperature = temperature;
        Precipitation = precipitation;
        Timestamp = timestamp;
        CreatedAt = DateTime.UtcNow;
    }
}