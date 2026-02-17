using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;

namespace AgroSolutions.Alerts.Application.Services;

public interface IAlertProcessingService
{
    Task ProcessSensorDataAsync(Guid fieldId, double soilMoisture, double temperature, double precipitation);
}

public class AlertProcessingService : IAlertProcessingService
{
    private readonly IAlertRepository _alertRepository;

    public AlertProcessingService(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task ProcessSensorDataAsync(Guid fieldId, double soilMoisture, double temperature, double precipitation)
    {
        var alerts = new List<Alert>();

        // Regra 1: Alerta de Seca (umidade < 30%)
        if (soilMoisture < 30)
        {
            var existingDroughtAlert = await CheckExistingActiveAlert(fieldId, "Seca");
            if (existingDroughtAlert == null)
            {
                alerts.Add(new Alert(
                    fieldId,
                    "Seca",
                    $"A umidade do solo está criticamente baixa em {soilMoisture}%. Recomenda-se irrigação.",
                    "Alta"));
            }
        }

        // Regra 2: Alerta de Temperatura Alta (> 35°C)
        if (temperature > 35)
        {
            var existingHeatAlert = await CheckExistingActiveAlert(fieldId, "AltaTemperatura");
            if (existingHeatAlert == null)
            {
                alerts.Add(new Alert(
                    fieldId,
                    "AltaTemperatura",
                    $"A temperatura está muito alta em {temperature}°C. Risco de estresse térmico para as culturas.",
                    "Média"));
            }
        }

        // Regra 3: Alerta de Precipitação Excessiva (> 50mm)
        if (precipitation > 50)
        {
            var existingFloodAlert = await CheckExistingActiveAlert(fieldId, "ChuvaForte");
            if (existingFloodAlert == null)
            {
                alerts.Add(new Alert(
                    fieldId,
                    "ChuvaForte",
                    $"Chuvas intensas detectadas: {precipitation}mm. Risco de inundações e erosão do solo.",
                    "Alta"));
            }
        }

        // Salvar todos os alertas
        foreach (var alert in alerts)
        {
            await _alertRepository.AddAsync(alert);
        }
    }

    private async Task<Alert?> CheckExistingActiveAlert(Guid fieldId, string type)
    {
        var activeAlerts = await _alertRepository.GetByFieldIdAsync(fieldId, isResolved: false);
        return activeAlerts.FirstOrDefault(a => a.Type == type);
    }
}