using AgroSolutions.Ingestion.Application.Events;
using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;
using MassTransit;

namespace AgroSolutions.Ingestion.Application.UseCases.ReceiveSensorData;

public record ReceiveSensorDataRequest(
    Guid FieldId,
    double SoilMoisture,
    double Temperature,
    double Precipitation,
    DateTime? Timestamp = null);

public record ReceiveSensorDataResponse(
    Guid SensorDataId,
    Guid FieldId,
    double SoilMoisture,
    double Temperature,
    double Precipitation,
    DateTime Timestamp,
    DateTime CreatedAt);

public class ReceiveSensorDataHandler
{
    private readonly ISensorDataRepository _sensorDataRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public ReceiveSensorDataHandler(ISensorDataRepository sensorDataRepository, IPublishEndpoint publishEndpoint)
    {
        _sensorDataRepository = sensorDataRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ReceiveSensorDataResponse> Handle(ReceiveSensorDataRequest request)
    {
        if (request.SoilMoisture < 0 || request.SoilMoisture > 100)
            throw new ArgumentException("A umidade do solo deve ser entre 0 e 100");

        if (request.Temperature < -50 || request.Temperature > 60)
            throw new ArgumentException("A temperatura deve ser de -50 a 60");

        if (request.Precipitation < 0)
            throw new ArgumentException("A precipitação não pode ser negativa");

        var timestamp = request.Timestamp ?? DateTime.UtcNow;

        var sensorData = new SensorData(
            request.FieldId,
            request.SoilMoisture,
            request.Temperature,
            request.Precipitation,
            timestamp);

        await _sensorDataRepository.AddAsync(sensorData);

        await _publishEndpoint.Publish(new SensorDataReceivedEvent(
            sensorData.Id,
            sensorData.FieldId,
            sensorData.SoilMoisture,
            sensorData.Temperature,
            sensorData.Precipitation,
            sensorData.Timestamp));

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