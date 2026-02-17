using AgroSolutions.Alerts.Application.Services;
using AgroSolutions.Ingestion.Application.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Alerts.Application.Events;

public class SensorDataReceivedConsumer : IConsumer<SensorDataReceivedEvent>
{
    private readonly IAlertProcessingService _alertProcessingService;
    private readonly ILogger<SensorDataReceivedConsumer> _logger;

    public SensorDataReceivedConsumer(
        IAlertProcessingService alertProcessingService,
        ILogger<SensorDataReceivedConsumer> logger)
    {
        _alertProcessingService = alertProcessingService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SensorDataReceivedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Consumindo evento SensorDataReceivedEvent - FieldId: {FieldId}, Umidade: {SoilMoisture}%",
            message.FieldId, message.SoilMoisture);

        try
        {
            await _alertProcessingService.ProcessSensorDataAsync(
                message.FieldId,
                message.SoilMoisture,
                message.Temperature,
                message.Precipitation);

            _logger.LogInformation("Processamento de alertas concluído com sucesso para FieldId: {FieldId}", message.FieldId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar alertas para FieldId: {FieldId}", message.FieldId);
            throw;
        }
    }
}