namespace ShaderService.Api.DTOs.Shaders;

public record GetShaderGenerationResponse(
    Guid GenerationId,
    string Status,
    string? FragmentShaderCode,
    string? VertexShaderCode,
    string? GeometryType
);