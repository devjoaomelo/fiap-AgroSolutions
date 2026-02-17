using AgroSolutions.Ingestion.Application.Events;
using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<ReceiveSensorDataHandler> _logger;

    public ReceiveSensorDataHandler(
        ISensorDataRepository sensorDataRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<ReceiveSensorDataHandler> logger)
    {
        _sensorDataRepository = sensorDataRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
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

        _logger.LogInformation("Publicando evento SensorDataReceivedEvent para FieldId: {FieldId}", sensorData.FieldId);

        try
        {
            await _publishEndpoint.Publish(new SensorDataReceivedEvent(
                sensorData.Id,
                sensorData.FieldId,
                sensorData.SoilMoisture,
                sensorData.Temperature,
                sensorData.Precipitation,
                sensorData.Timestamp));

            _logger.LogInformation("Evento publicado com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar evento no RabbitMQ");
            throw;
        }

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