using AgroSolutions.Property.Domain.Entities;
using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.CreateRuralProperty;

public record CreateRuralPropertyRequest(Guid UserId, string Name, string Location, double TotalArea);

public record CreateRuralPropertyResponse(Guid Id, string Name, string Location, double TotalArea, DateTime CreatedAt);

public class CreateRuralPropertyHandler
{
    private readonly IRuralPropertyRepository _propertyRepository;

    public CreateRuralPropertyHandler(IRuralPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<CreateRuralPropertyResponse> Handle(CreateRuralPropertyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Location))
            throw new ArgumentException("Local é obrigatório");

        if (request.TotalArea <= 0)
            throw new ArgumentException("Área total deve ser maior que zero");

        var property = new RuralProperty(request.UserId, request.Name, request.Location, request.TotalArea);

        await _propertyRepository.AddAsync(property);

        return new CreateRuralPropertyResponse(property.Id, property.Name, property.Location, property.TotalArea, property.CreatedAt);
    }
}