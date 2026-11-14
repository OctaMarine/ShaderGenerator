namespace ShaderService.Application.Abstractions;

public interface IShaderGenerationPublisher
{
    Task PublishAsync(
        Guid generationId,
        Guid userId,
        string prompt,
        CancellationToken cancellationToken);
}