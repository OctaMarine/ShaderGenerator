using Microsoft.AspNetCore.Mvc;
using ShaderService.Api.DTOs.Shaders;
using ShaderService.Application;
using ShaderService.Application.ShaderGenerations;
using ShaderService.Domain.Entities;

[ApiController]
[Route("api/shaders")]
public class ShaderController : ControllerBase
{
    
    private readonly ShaderGeneratorService _shaderGeneratorService;
    private readonly GetShaderGenerationService _getShaderGenerationService;

    public ShaderController(
        ShaderGeneratorService shaderGeneratorService,
        GetShaderGenerationService getShaderGenerationService)
    {
        _shaderGeneratorService = shaderGeneratorService;
        _getShaderGenerationService = getShaderGenerationService;
    }
    
    [HttpPost("generate")]
    public async Task<ActionResult<GenerateShaderResponse>> GenerateShader(
        [FromBody] GenerateShaderRequest request,
        CancellationToken cancellationToken)
    {
        var generationId = await _shaderGeneratorService.GenerateAsync(
            request.UserId,
            request.Prompt,
            cancellationToken);

        var response = new GenerateShaderResponse(
            generationId,
            "Pending");

        return Accepted(response);
    }
    
    [HttpGet("{generationId:guid}")]
    public async Task<ActionResult<GetShaderGenerationResponse>> GetShader(
        Guid generationId,
        CancellationToken cancellationToken)
    {
        var generation = await _getShaderGenerationService.GetByIdAsync(
            generationId,
            cancellationToken);

        if (generation is null)
        {
            return NotFound();
        }

        var response = new GetShaderGenerationResponse(
            generation.Id,
            generation.Status.ToString(),
            generation.FragmentShaderCode,
            generation.VertexShaderCode,
            generation.GeometryType);

        return Ok(response);
    }
}