using AgroSolutions.Property.Domain.Entities;

namespace AgroSolutions.Property.Domain.Interfaces;

public interface IRuralPropertyRepository
{
    Task<RuralProperty?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<RuralProperty>> GetByUserIdAsync(Guid userId);
    Task AddAsync(RuralProperty property);
    Task UpdateAsync(RuralProperty property);
    Task DeleteAsync(Guid id);
}