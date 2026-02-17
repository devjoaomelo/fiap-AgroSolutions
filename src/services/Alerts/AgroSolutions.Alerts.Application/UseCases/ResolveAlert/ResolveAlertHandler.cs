using AgroSolutions.Alerts.Domain.Interfaces;

namespace AgroSolutions.Alerts.Application.UseCases.ResolveAlert;

public record ResolveAlertRequest(Guid AlertId);

public record ResolveAlertResponse(Guid AlertId, bool IsResolved, DateTime ResolvedAt);

public class ResolveAlertHandler
{
    private readonly IAlertRepository _alertRepository;

    public ResolveAlertHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<ResolveAlertResponse> Handle(ResolveAlertRequest request)
    {
        var alert = await _alertRepository.GetByIdAsync(request.AlertId);

        if (alert == null)
            throw new InvalidOperationException("Alerta não encontrado");

        if (alert.IsResolved)
            throw new InvalidOperationException("Alerta já foi resolvido");

        alert.Resolve();

        await _alertRepository.UpdateAsync(alert);

        return new ResolveAlertResponse(alert.Id, alert.IsResolved, alert.ResolvedAt!.Value);
    }
}