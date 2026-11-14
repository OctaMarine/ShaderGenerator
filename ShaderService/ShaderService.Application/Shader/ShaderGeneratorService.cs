using ShaderService.Application.Abstractions;
using ShaderService.Domain.Entities;
using ShaderService.Domain.Enums;

namespace ShaderService.Application;

public class ShaderGeneratorService
{
    private readonly IShaderRepository _shaderRepository;
    private readonly IShaderGenerationPublisher _publisher;

    public ShaderGeneratorService(
        IShaderRepository shaderRepository,
        IShaderGenerationPublisher publisher)
    {
        _shaderRepository = shaderRepository;
        _publisher = publisher;
    }
    
    public async Task<Guid> CreateAsync(Guid userId,
        string prompt,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("Invalid user ID.");

        if (string.IsNullOrWhiteSpace(prompt))
            throw new ArgumentException("Prompt cannot be empty.");
        
        var generation = new ShaderGeneration
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Prompt = prompt.Trim(),
            Status = ShaderGenerationStatus.Pending,
            CreatedAt = DateTime.Now
        };
        
        await _shaderRepository.AddAsync(
            generation,
            cancellationToken);
        await _publisher.PublishAsync(
            generation.Id,
            generation.UserId,
            generation.Prompt,
            cancellationToken);

        return generation.Id;
    }
}