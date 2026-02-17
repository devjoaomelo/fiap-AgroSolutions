namespace AgroSolutions.Alerts.Application.UseCases.ProcessSensorData;

public record ProcessSensorDataRequest(Guid FieldId, double SoilMoisture, double Temperature, double Precipitation);