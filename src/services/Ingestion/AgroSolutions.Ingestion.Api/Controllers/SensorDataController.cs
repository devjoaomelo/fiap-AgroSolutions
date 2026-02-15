using AgroSolutions.Ingestion.Application.UseCases.GetSensorData;
using AgroSolutions.Ingestion.Application.UseCases.ReceiveSensorData;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Ingestion.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorDataController : ControllerBase
{
    private readonly ReceiveSensorDataHandler _receiveSensorDataHandler;
    private readonly GetSensorDataHandler _getSensorDataHandler;

    public SensorDataController(
        ReceiveSensorDataHandler receiveSensorDataHandler,
        GetSensorDataHandler getSensorDataHandler)
    {
        _receiveSensorDataHandler = receiveSensorDataHandler;
        _getSensorDataHandler = getSensorDataHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Receive([FromBody] ReceiveSensorDataRequest request)
    {
        try
        {
            var response = await _receiveSensorDataHandler.Handle(request);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("field/{fieldId}")]
    public async Task<IActionResult> GetByField(
        Guid fieldId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var response = await _getSensorDataHandler.Handle(
            new GetSensorDataRequest(fieldId, startDate, endDate));

        return Ok(response);
    }
}