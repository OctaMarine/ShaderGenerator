namespace ShaderService.Api.DTOs.Shaders;

public record GenerateShaderResponse(
    Guid GenerationId,
    string Status);