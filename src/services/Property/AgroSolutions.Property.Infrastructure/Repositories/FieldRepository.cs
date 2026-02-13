using AgroSolutions.Property.Domain.Entities;
using AgroSolutions.Property.Domain.Interfaces;
using AgroSolutions.Property.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Property.Infrastructure.Repositories;

public class FieldRepository : IFieldRepository
{
    private readonly RuralPropertyDbContext _context;

    public FieldRepository(RuralPropertyDbContext context)
    {
        _context = context;
    }

    public async Task<Field?> GetByIdAsync(Guid id)
    {
        return await _context.Fields
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<IReadOnlyList<Field>> GetByPropertyIdAsync(Guid propertyId)
    {
        return await _context.Fields
            .Where(f => f.RuralPropertyId == propertyId)
            .ToListAsync();
    }

    public async Task AddAsync(Field field)
    {
        await _context.Fields.AddAsync(field);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Field field)
    {
        _context.Fields.Update(field);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var field = await _context.Fields.FindAsync(id);
        if (field != null)
        {
            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();
        }
    }
}