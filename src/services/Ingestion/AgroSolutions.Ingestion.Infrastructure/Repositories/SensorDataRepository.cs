using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;
using AgroSolutions.Ingestion.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Ingestion.Infrastructure.Repositories;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly IngestionDbContext _context;

    public SensorDataRepository(IngestionDbContext context)
    {
        _context = context;
    }

    public async Task<SensorData?> GetByIdAsync(Guid id)
    {
        return await _context.SensorData.FindAsync(id);
    }

    public async Task<IReadOnlyList<SensorData>> GetByFieldIdAsync(Guid fieldId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.SensorData.Where(s => s.FieldId == fieldId);

        if (startDate.HasValue)
            query = query.Where(s => s.Timestamp >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.Timestamp <= endDate.Value);

        return await query.OrderByDescending(s => s.Timestamp).ToListAsync();
    }

    public async Task AddAsync(SensorData sensorData)
    {
        await _context.SensorData.AddAsync(sensorData);
        await _context.SaveChangesAsync();
    }
}