using AgroSolutions.Ingestion.Domain.Entities;

namespace AgroSolutions.Ingestion.Domain.Interfaces;

public interface ISensorDataRepository
{
    Task<SensorData?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<SensorData>> GetByFieldIdAsync(Guid fieldId, DateTime? startDate = null, DateTime? endDate = null);
    Task AddAsync(SensorData sensorData);
}