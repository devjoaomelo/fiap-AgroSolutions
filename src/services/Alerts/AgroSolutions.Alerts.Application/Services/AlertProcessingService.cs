using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AgroSolutions.Alerts.Application.Services;

public interface IAlertProcessingService
{
    Task ProcessSensorDataAsync(Guid fieldId, double soilMoisture, double temperature, double precipitation);
}

public class AlertProcessingService : IAlertProcessingService
{
    private readonly IAlertRepository _alertRepository;
    private readonly ILogger<AlertProcessingService> _logger;

    public AlertProcessingService(IAlertRepository alertRepository, ILogger<AlertProcessingService> logger)
    {
        _alertRepository = alertRepository;
        _logger = logger;
    }

    public async Task ProcessSensorDataAsync(Guid fieldId, double soilMoisture, double temperature, double precipitation)
    {
        _logger.LogInformation("Processando dados do sensor - FieldId: {FieldId}, Umidade: {SoilMoisture}%, Temp: {Temperature}°C, Precip: {Precipitation}mm",
            fieldId, soilMoisture, temperature, precipitation);

        var alerts = new List<Alert>();

        // Regra 1: Alerta de Seca (umidade < 30%)
        if (soilMoisture < 30)
        {
            _logger.LogInformation("Umidade baixa detectada: {SoilMoisture}%", soilMoisture);
            var existingDroughtAlert = await CheckExistingActiveAlert(fieldId, "Seca");
            if (existingDroughtAlert == null)
            {
                alerts.Add(new Alert(
                    fieldId,
                    "Seca",
                    $"A umidade do solo está criticamente baixa em {soilMoisture}%. Recomenda-se irrigação.",
                    "Alta"));
                _logger.LogInformation("Alerta de Seca criado");
            }
            else
            {
                _logger.LogInformation("Alerta de Seca já existe, não criando duplicado");
            }
        }

        // Regra 2: Alerta de Temperatura Alta (> 35°C)
        if (temperature > 35)
        {
            _logger.LogInformation("Temperatura alta detectada: {Temperature}°C", temperature);
            var existingHeatAlert = await CheckExistingActiveAlert(fieldId, "AltaTemperatura");
            if (existingHeatAlert == null)
            {
                alerts.Add(new Alert(
                    fieldId,
                    "AltaTemperatura",
                    $"A temperatura está muito alta em {temperature}°C. Risco de estresse térmico para as culturas.",
                    "Média"));
                _logger.LogInformation("Alerta de Alta Temperatura criado");
            }
            else
            {
                _logger.LogInformation("Alerta de Alta Temperatura já existe, não criando duplicado");
            }
        }

        // Regra 3: Alerta de Precipitação Excessiva (> 50mm)
        if (precipitation > 50)
        {
            _logger.LogInformation("Precipitação alta detectada: {Precipitation}mm", precipitation);
            var existingFloodAlert = await CheckExistingActiveAlert(fieldId, "ChuvaForte");
            if (existingFloodAlert == null)
            {
                alerts.Add(new Alert(
                    fieldId,
                    "ChuvaForte",
                    $"Chuvas intensas detectadas: {precipitation}mm. Risco de inundações e erosão do solo.",
                    "Alta"));
                _logger.LogInformation("Alerta de Chuva Forte criado");
            }
            else
            {
                _logger.LogInformation("Alerta de Chuva Forte já existe, não criando duplicado");
            }
        }

        _logger.LogInformation("Total de alertas a serem salvos: {Count}", alerts.Count);

        // Salvar todos os alertas
        foreach (var alert in alerts)
        {
            await _alertRepository.AddAsync(alert);
            _logger.LogInformation("Alerta salvo: {Type} para FieldId: {FieldId}", alert.Type, alert.FieldId);
        }

        _logger.LogInformation("Processamento de alertas concluído");
    }

    private async Task<Alert?> CheckExistingActiveAlert(Guid fieldId, string type)
    {
        var activeAlerts = await _alertRepository.GetByFieldIdAsync(fieldId, isResolved: false);
        return activeAlerts.FirstOrDefault(a => a.Type == type);
    }
}