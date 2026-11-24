using ShaderService.Application.Abstractions;
using ShaderService.Domain.Entities;

namespace ShaderService.Application.ShaderGenerations;

public class GetShaderGenerationService
{
    private readonly IShaderRepository _shaderRepository;

    public GetShaderGenerationService(
        IShaderRepository shaderRepository)
    {
        _shaderRepository = shaderRepository;
    }

    public Task<ShaderGeneration?> GetByIdAsync(
        Guid generationId,
        CancellationToken cancellationToken)
    {
        return _shaderRepository.GetByIdAsync(
            generationId,
            cancellationToken);
    }
}