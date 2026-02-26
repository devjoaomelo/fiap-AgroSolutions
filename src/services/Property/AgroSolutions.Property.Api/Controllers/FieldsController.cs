using AgroSolutions.Property.Application.UseCases.CreateField;
using AgroSolutions.Property.Application.UseCases.GetFieldsByProperty;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Property.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FieldsController : ControllerBase
{
    private readonly CreateFieldHandler _createFieldHandler;
    private readonly GetFieldsByPropertyHandler _getFieldsByPropertyHandler;

    public FieldsController(
        CreateFieldHandler createFieldHandler,
        GetFieldsByPropertyHandler getFieldsByPropertyHandler)
    {
        _createFieldHandler = createFieldHandler;
        _getFieldsByPropertyHandler = getFieldsByPropertyHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFieldRequest request)
    {
        try
        {
            var response = await _createFieldHandler.Handle(request);
            return CreatedAtAction(nameof(Create), new { id = response.FieldId }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetByProperty(Guid propertyId)
    {
        var response = await _getFieldsByPropertyHandler.Handle(new GetFieldsByPropertyRequest(propertyId));
        return Ok(response);
    }
}