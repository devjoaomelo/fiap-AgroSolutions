namespace AgroSolutions.Ingestion.Application.Events;

public record SensorDataReceivedEvent(
    Guid SensorDataId,
    Guid FieldId,
    double SoilMoisture,
    double Temperature,
    double Precipitation,
    DateTime Timestamp);