using System.ComponentModel.DataAnnotations;

namespace ShaderService.Api.DTOs.Shaders;

public record GenerateShaderRequest(
    [Required] Guid UserId,
    [Required] string Prompt);