using AgroSolutions.Alerts.Domain.Interfaces;

namespace AgroSolutions.Alerts.Application.UseCases.GetAlerts;

public record GetAlertsRequest(Guid? FieldId = null, bool? IsResolved = null);

public record AlertDto(Guid AlertId, Guid FieldId, string Type, string Message, string Severity, DateTime CreatedAt, bool IsResolved, DateTime? ResolvedAt);

public record GetAlertsResponse(List<AlertDto> Alerts);

public class GetAlertsHandler
{
    private readonly IAlertRepository _alertRepository;

    public GetAlertsHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<GetAlertsResponse> Handle(GetAlertsRequest request)
    {
        var alerts = request.FieldId.HasValue
            ? await _alertRepository.GetByFieldIdAsync(request.FieldId.Value, request.IsResolved)
            : await _alertRepository.GetActiveAlertsAsync();

        var alertsDto = alerts.Select(a => new AlertDto(
            a.Id,
            a.FieldId,
            a.Type,
            a.Message,
            a.Severity,
            a.CreatedAt,
            a.IsResolved,
            a.ResolvedAt
        )).ToList();

        return new GetAlertsResponse(alertsDto);
    }
}