namespace Contracts.ShaderGeneration;

public sealed record GenerateShaderCommand(
    Guid GenerationId,
    Guid UserId,
    string Prompt
);