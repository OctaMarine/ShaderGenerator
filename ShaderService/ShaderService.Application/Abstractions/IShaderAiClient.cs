namespace ShaderService.Application.Abstractions;

public interface IShaderAiClient
{
    Task<GeneratedShaderResult> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken);
    
}

public record GeneratedShaderResult(
    string VertexShaderCode,
    string FragmentShaderCode,
    string GeometryType
);