using ShaderService.Domain.Enums;

namespace ShaderService.Domain.Entities;

public class ShaderGeneration
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ShaderGenerationStatus Status { get; set; }
    public string? Prompt { get; set; }
    public string? ShaderCode { get; set; }
    public DateTime CreatedAt { get; set; }
}