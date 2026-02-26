using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.GetFieldsByProperty;

public sealed record GetFieldsByPropertyRequest(Guid RuralPropertyId);

public sealed record FieldDto(
    Guid FieldId,
    Guid RuralPropertyId,
    string Name,
    string Culture,
    double Area
);

public sealed record GetFieldsByPropertyResponse(List<FieldDto> Fields);

public sealed class GetFieldsByPropertyHandler
{
    private readonly IFieldRepository _fieldRepository;

    public GetFieldsByPropertyHandler(IFieldRepository fieldRepository)
    {
        _fieldRepository = fieldRepository;
    }

    public async Task<GetFieldsByPropertyResponse> Handle(GetFieldsByPropertyRequest request)
    {
        var fields = await _fieldRepository.GetByPropertyIdAsync(request.RuralPropertyId);

        var fieldDtos = fields.Select(f => new FieldDto(
            f.Id,
            f.RuralPropertyId,
            f.Name,
            f.Culture,
            f.Area
        )).ToList();

        return new GetFieldsByPropertyResponse(fieldDtos);
    }
}