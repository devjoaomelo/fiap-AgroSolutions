using AgroSolutions.Property.Domain.Entities;
using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.CreateField;

public record CreateFieldRequest(Guid RuralPropertyId, string Name, string Culture, double Area);

public record CreateFieldResponse(Guid Id, Guid RuralPropertyId, string Name, string Culture, double Area, DateTime CreatedAt);

public class CreateFieldHandler
{
    private readonly IFieldRepository _fieldRepository;
    private readonly IRuralPropertyRepository _ruralPropertyRepository;

    public CreateFieldHandler(IFieldRepository fieldRepository, IRuralPropertyRepository ruralPropertyRepository)
    {
        _fieldRepository = fieldRepository;
        _ruralPropertyRepository = ruralPropertyRepository;
    }

    public async Task<CreateFieldResponse> Handle(CreateFieldRequest request)
    {
        var property = await _ruralPropertyRepository.GetByIdAsync(request.RuralPropertyId);
        if (property == null)
            throw new InvalidOperationException("Propriedade não encontrada");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Culture))
            throw new ArgumentException("A Cultura é obrigatória");

        if (request.Area <= 0)
            throw new ArgumentException("Área deve ser maior que zero");

        var field = new Field(request.RuralPropertyId, request.Name, request.Culture, request.Area);

        await _fieldRepository.AddAsync(field);

        return new CreateFieldResponse(field.Id, field.RuralPropertyId, field.Name, field.Culture, field.Area, field.CreatedAt);
    }
}