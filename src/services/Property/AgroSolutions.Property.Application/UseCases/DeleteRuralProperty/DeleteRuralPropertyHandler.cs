using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.DeleteRuralProperty;

public sealed record DeleteRuralPropertyRequest(Guid PropertyId);
public sealed record DeleteRuralPropertyResponse(bool Success);

public sealed class DeleteRuralPropertyHandler
{
    private readonly IRuralPropertyRepository _propertyRepository;
    private readonly IFieldRepository _fieldRepository;

    public DeleteRuralPropertyHandler(
        IRuralPropertyRepository propertyRepository,
        IFieldRepository fieldRepository)
    {
        _propertyRepository = propertyRepository;
        _fieldRepository = fieldRepository;
    }

    public async Task<DeleteRuralPropertyResponse> Handle(DeleteRuralPropertyRequest request)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (property == null)
            throw new KeyNotFoundException("Propriedade não encontrada");

        var fields = await _fieldRepository.GetByPropertyIdAsync(request.PropertyId);
        foreach (var field in fields)
        {
            await _fieldRepository.DeleteAsync(field.Id);
        }

        await _propertyRepository.DeleteAsync(request.PropertyId);

        return new DeleteRuralPropertyResponse(true);
    }
}