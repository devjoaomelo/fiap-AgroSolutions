using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;

namespace AgroSolutions.Alerts.Application.UseCases.CreateAlert;

public record CreateAlertRequest(Guid FieldId, string Type, string Message, string Severity);

public record CreateAlertResponse(Guid AlertId, Guid FieldId, string Type, string Message, string Severity, DateTime CreatedAt);

public class CreateAlertHandler
{
    private readonly IAlertRepository _alertRepository;

    public CreateAlertHandler(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    public async Task<CreateAlertResponse> Handle(CreateAlertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Type))
            throw new ArgumentException("Tipo é obrigatório");

        if (string.IsNullOrWhiteSpace(request.Message))
            throw new ArgumentException("Mensagem é obrigatória");

        if (string.IsNullOrWhiteSpace(request.Severity))
            throw new ArgumentException("Severidade é obrigatória");

        var alert = new Alert(request.FieldId, request.Type, request.Message, request.Severity);

        await _alertRepository.AddAsync(alert);

        return new CreateAlertResponse(
            alert.Id,
            alert.FieldId,
            alert.Type,
            alert.Message,
            alert.Severity,
            alert.CreatedAt);
    }
}