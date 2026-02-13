using AgroSolutions.Property.Domain.Interfaces;

namespace AgroSolutions.Property.Application.UseCases.GetProperties;

public record GetPropertiesRequest(Guid UserId);

public record RuralPropertyDto(Guid Id, string Name, string Location, double TotalArea, int FieldsCount);

public record GetPropertiesResponse(List<RuralPropertyDto> Properties);

public class GetPropertiesHandler
{
    private readonly IRuralPropertyRepository _ruralPropertyRepository;

    public GetPropertiesHandler(IRuralPropertyRepository ruralPropertyRepository)
    {
        _ruralPropertyRepository = ruralPropertyRepository;
    }

    public async Task<GetPropertiesResponse> Handle(GetPropertiesRequest request)
    {
        var ruralProperties = await _ruralPropertyRepository.GetByUserIdAsync(request.UserId);

        var ruralPropertiesDto = ruralProperties.Select(p => new RuralPropertyDto(
            p.Id,
            p.Name,
            p.Location,
            p.TotalArea,
            p.Fields.Count
        )).ToList();

        return new GetPropertiesResponse(ruralPropertiesDto);
    }
}