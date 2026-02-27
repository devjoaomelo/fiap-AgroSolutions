using AgroSolutions.Property.Application.UseCases.CreateRuralProperty;
using AgroSolutions.Property.Application.UseCases.DeleteRuralProperty;
using AgroSolutions.Property.Application.UseCases.GetRuralProperties;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Property.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly CreateRuralPropertyHandler _createRuralPropertyHandler;
    private readonly GetRuralPropertiesHandler _getRuralPropertiesHandler;
    private readonly DeleteRuralPropertyHandler _deleteRuralPropertyHandler;

    public PropertiesController(
        CreateRuralPropertyHandler createPropertyHandler,
        GetRuralPropertiesHandler getRuralPropertiesHandler,
        DeleteRuralPropertyHandler deleteRuralPropertyHandler)
    {
        _createRuralPropertyHandler = createPropertyHandler;
        _getRuralPropertiesHandler = getRuralPropertiesHandler;
        _deleteRuralPropertyHandler = deleteRuralPropertyHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRuralPropertyRequest request)
    {
        try
        {
            var response = await _createRuralPropertyHandler.Handle(request);
            return CreatedAtAction(nameof(GetByUser), new { userId = request.UserId }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(Guid userId)
    {
        var response = await _getRuralPropertiesHandler.Handle(new GetRuralPropertiesRequest(userId));
        return Ok(response);
    }

    [HttpDelete("{propertyId}")]
    public async Task<IActionResult> Delete(Guid propertyId)
    {
        try
        {
            var response = await _deleteRuralPropertyHandler.Handle(new DeleteRuralPropertyRequest(propertyId));
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}