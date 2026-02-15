using AgroSolutions.Alerts.Domain.Entities;

namespace AgroSolutions.Alerts.Domain.Interfaces;

public interface IAlertRepository
{
    Task<Alert?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Alert>> GetByFieldIdAsync(Guid fieldId, bool? isResolved = null);
    Task<IReadOnlyList<Alert>> GetActiveAlertsAsync();
    Task AddAsync(Alert alert);
    Task UpdateAsync(Alert alert);
}