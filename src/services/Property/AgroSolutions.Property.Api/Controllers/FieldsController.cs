using AgroSolutions.Property.Application.UseCases.CreateField;
using Microsoft.AspNetCore.Mvc;

namespace AgroSolutions.Property.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FieldsController : ControllerBase
{
    private readonly CreateFieldHandler _createFieldHandler;

    public FieldsController(CreateFieldHandler createFieldHandler)
    {
        _createFieldHandler = createFieldHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFieldRequest request)
    {
        try
        {
            var response = await _createFieldHandler.Handle(request);
            return CreatedAtAction(nameof(Create), new { id = response.Id }, response);
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
}