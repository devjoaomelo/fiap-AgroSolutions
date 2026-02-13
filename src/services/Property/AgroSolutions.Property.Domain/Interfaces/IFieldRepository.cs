using AgroSolutions.Property.Domain.Entities;

namespace AgroSolutions.Property.Domain.Interfaces;

public interface IFieldRepository
{
    Task<Field?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Field>> GetByPropertyIdAsync(Guid propertyId);
    Task AddAsync(Field field);
    Task UpdateAsync(Field field);
    Task DeleteAsync(Guid id);
}