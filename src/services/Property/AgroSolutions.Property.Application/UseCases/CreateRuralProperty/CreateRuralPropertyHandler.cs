using AgroSolutions.Property.Domain.Entities;
using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.CreateRuralProperty;

public record CreateRuralPropertyRequest(Guid UserId, string Name, string Location, double TotalArea);

public record CreateRuralPropertyResponse(Guid PropertyId, string Name, string Location, double TotalArea, DateTime CreatedAt);

public class CreateRuralPropertyHandler
{
    private readonly IRuralPropertyRepository _ruralPropertyRepository;

    public CreateRuralPropertyHandler(IRuralPropertyRepository ruralPropertyRepository)
    {
        _ruralPropertyRepository = ruralPropertyRepository;
    }

    public async Task<CreateRuralPropertyResponse> Handle(CreateRuralPropertyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Nome é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Location))
            throw new ArgumentException("Localização é obrigatória");

        if (request.TotalArea <= 0)
            throw new ArgumentException("Área total deve ser maior que zero");

        var ruralProperty = new RuralProperty(request.UserId, request.Name, request.Location, request.TotalArea);

        await _ruralPropertyRepository.AddAsync(ruralProperty);

        return new CreateRuralPropertyResponse(ruralProperty.Id, ruralProperty.Name, ruralProperty.Location, ruralProperty.TotalArea, ruralProperty.CreatedAt);
    }
}