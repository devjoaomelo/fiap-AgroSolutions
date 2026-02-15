using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Domain.Interfaces;
using AgroSolutions.Alerts.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Alerts.Infrastructure.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly AlertsDbContext _context;

    public AlertRepository(AlertsDbContext context)
    {
        _context = context;
    }

    public async Task<Alert?> GetByIdAsync(Guid id)
    {
        return await _context.Alerts.FindAsync(id);
    }

    public async Task<IReadOnlyList<Alert>> GetByFieldIdAsync(Guid fieldId, bool? isResolved = null)
    {
        var query = _context.Alerts.Where(a => a.FieldId == fieldId);

        if (isResolved.HasValue)
            query = query.Where(a => a.IsResolved == isResolved.Value);

        return await query.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public async Task<IReadOnlyList<Alert>> GetActiveAlertsAsync()
    {
        return await _context.Alerts
            .Where(a => !a.IsResolved)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Alert alert)
    {
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Alert alert)
    {
        _context.Alerts.Update(alert);
        await _context.SaveChangesAsync();
    }
}