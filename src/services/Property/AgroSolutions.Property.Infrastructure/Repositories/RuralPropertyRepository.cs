using AgroSolutions.Property.Domain.Entities;
using AgroSolutions.Property.Domain.Interfaces;
using AgroSolutions.Property.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Property.Infrastructure.Repositories;

public class RuralPropertyRepository : IPropertyRepository
{
    private readonly RuralPropertyDbContext _context;

    public RuralPropertyRepository(RuralPropertyDbContext context)
    {
        _context = context;
    }

    public async Task<RuralProperty?> GetByIdAsync(Guid id)
    {
        return await _context.RuralProperties
            .Include(p => p.Fields)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyList<RuralProperty>> GetByUserIdAsync(Guid userId)
    {
        return await _context.RuralProperties
            .Include(p => p.Fields)
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(RuralProperty property)
    {
        await _context.RuralProperties.AddAsync(property);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RuralProperty property)
    {
        _context.RuralProperties.Update(property);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var property = await _context.RuralProperties.FindAsync(id);
        if (property != null)
        {
            _context.RuralProperties.Remove(property);
            await _context.SaveChangesAsync();
        }
    }
}