using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;

namespace AgroSolutions.Ingestion.Application.UseCases.ReceiveSensorData;

public record ReceiveSensorDataRequest(
    Guid FieldId,
    double SoilMoisture,
    double Temperature,
    double Precipitation,
    DateTime? Timestamp = null);

public record ReceiveSensorDataResponse(
    Guid Id,
    Guid FieldId,
    double SoilMoisture,
    double Temperature,
    double Precipitation,
    DateTime Timestamp,
    DateTime CreatedAt);

public class ReceiveSensorDataHandler
{
    private readonly ISensorDataRepository _sensorDataRepository;

    public ReceiveSensorDataHandler(ISensorDataRepository sensorDataRepository)
    {
        _sensorDataRepository = sensorDataRepository;
    }

    public async Task<ReceiveSensorDataResponse> Handle(ReceiveSensorDataRequest request)
    {
        if (request.SoilMoisture < 0 || request.SoilMoisture > 100)
            throw new ArgumentException("Umidade deve estar entre 0 e 100");

        if (request.Temperature < -50 || request.Temperature > 60)
            throw new ArgumentException("Temperatura deve estar entre -50 e 60");

        if (request.Precipitation < 0)
            throw new ArgumentException("Precipitação não pode ser negativa");

        var timestamp = request.Timestamp ?? DateTime.UtcNow;

        var sensorData = new SensorData(
            request.FieldId,
            request.SoilMoisture,
            request.Temperature,
            request.Precipitation,
            timestamp);

        await _sensorDataRepository.AddAsync(sensorData);

        return new ReceiveSensorDataResponse(
            sensorData.Id,
            sensorData.FieldId,
            sensorData.SoilMoisture,
            sensorData.Temperature,
            sensorData.Precipitation,
            sensorData.Timestamp,
            sensorData.CreatedAt);
    }
}