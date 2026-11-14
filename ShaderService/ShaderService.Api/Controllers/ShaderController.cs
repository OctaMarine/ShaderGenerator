using Microsoft.AspNetCore.Mvc;
using ShaderService.Application;
using ShaderService.Domain.Entities;

[ApiController]
[Route("api/shaders")]
public class ShaderController : ControllerBase
{
    
    private readonly ShaderGeneratorService _shaderGeneratorService;

    public ShaderController(ShaderGeneratorService shaderGeneratorService)
    {
        _shaderGeneratorService = shaderGeneratorService;
    }
    
    [HttpPost("generate")]
    public async Task<ActionResult<ShaderGeneration>> GenerateShader(Guid userId, string prompt)
    {
        return Ok(await _shaderGeneratorService.CreateAsync(userId, prompt, new CancellationToken()));
    }
}