using Microsoft.EntityFrameworkCore;
using ShaderService.Application.Abstractions;
using ShaderService.Domain.Entities;

namespace ShaderService.Infrastructure;

public class ShaderRepository : IShaderRepository
{
    private readonly AppDbContext _context;

    public ShaderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        ShaderGeneration generation,
        CancellationToken cancellationToken)
    {
        _context.ShaderGenerations.Add(generation);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ShaderGeneration?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.ShaderGenerations
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task UpdateAsync(
        ShaderGeneration generation,
        CancellationToken cancellationToken)
    {
        _context.ShaderGenerations.Update(generation);

        await _context.SaveChangesAsync(cancellationToken);
    }
}