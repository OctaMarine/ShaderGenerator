using ShaderService.Domain.Entities;

namespace ShaderService.Application.Abstractions;

public interface IShaderRepository
{
    Task AddAsync(
        ShaderGeneration generation,
        CancellationToken cancellationToken);

    Task<ShaderGeneration?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        ShaderGeneration generation,
        CancellationToken cancellationToken);
}