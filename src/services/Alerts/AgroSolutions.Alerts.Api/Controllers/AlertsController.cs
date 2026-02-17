using AgroSolutions.Alerts.Application.Services;
using AgroSolutions.Alerts.Application.UseCases.CreateAlert;
using AgroSolutions.Alerts.Application.UseCases.GetAlerts;
using AgroSolutions.Alerts.Application.UseCases.ResolveAlert;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Alerts.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly CreateAlertHandler _createAlertHandler;
    private readonly GetAlertsHandler _getAlertsHandler;
    private readonly ResolveAlertHandler _resolveAlertHandler;
    private readonly IAlertProcessingService _alertProcessingService;

    public AlertsController(
        CreateAlertHandler createAlertHandler,
        GetAlertsHandler getAlertsHandler,
        ResolveAlertHandler resolveAlertHandler,
        IAlertProcessingService alertProcessingService)
    {
        _createAlertHandler = createAlertHandler;
        _getAlertsHandler = getAlertsHandler;
        _resolveAlertHandler = resolveAlertHandler;
        _alertProcessingService = alertProcessingService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlertRequest request)
    {
        try
        {
            var response = await _createAlertHandler.Handle(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? fieldId = null, [FromQuery] bool? isResolved = null)
    {
        var response = await _getAlertsHandler.Handle(new GetAlertsRequest(fieldId, isResolved));
        return Ok(response);
    }

    [HttpPut("{id}/resolve")]
    public async Task<IActionResult> Resolve(Guid id)
    {
        try
        {
            var response = await _resolveAlertHandler.Handle(new ResolveAlertRequest(id));
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessSensorData([FromBody] ProcessSensorDataRequest request)
    {
        await _alertProcessingService.ProcessSensorDataAsync(
            request.FieldId,
            request.SoilMoisture,
            request.Temperature,
            request.Precipitation);

        return Ok(new { message = "Dados do sensor processados com sucesso" });
    }
}

public record ProcessSensorDataRequest(Guid FieldId, double SoilMoisture, double Temperature, double Precipitation);