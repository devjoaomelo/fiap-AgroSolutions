using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.GetRuralProperties;

public record GetRuralPropertiesRequest(Guid UserId);

public record RuralPropertyDto(Guid Id, string Name, string Location, double TotalArea, int FieldsCount);

public record GetRuralPropertiesResponse(List<RuralPropertyDto> Properties);

public class GetRuralPropertiesHandler
{
    private readonly IRuralPropertyRepository _ruralPropertyRepository;

    public GetRuralPropertiesHandler(IRuralPropertyRepository ruralPropertyRepository)
    {
        _ruralPropertyRepository = ruralPropertyRepository;
    }

    public async Task<GetRuralPropertiesResponse> Handle(GetRuralPropertiesRequest request)
    {
        var ruralProperties = await _ruralPropertyRepository.GetByUserIdAsync(request.UserId);

        var ruralPropertiesDto = ruralProperties.Select(p => new RuralPropertyDto(
            p.Id,
            p.Name,
            p.Location,
            p.TotalArea,
            p.Fields.Count
        )).ToList();

        return new GetRuralPropertiesResponse(ruralPropertiesDto);
    }
}