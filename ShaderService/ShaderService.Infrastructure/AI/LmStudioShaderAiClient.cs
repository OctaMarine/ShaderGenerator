using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using ShaderService.Application.Abstractions;

namespace ShaderService.Infrastructure.AI;

public class LmStudioShaderAiClient : IShaderAiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _model;

    public LmStudioShaderAiClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _model = configuration["LmStudio:Model"]
            ?? throw new InvalidOperationException(
                "LmStudio:Model is not configured.");
    }

    public async Task<GeneratedShaderResult> GenerateAsync(
        string prompt,
        CancellationToken cancellationToken)
    {
        var request = new
        {
            model = _model,
            messages = new[]
            {
                new
                {
                    role = "system",
                    
                    content = @"
You are an expert GLSL shader developer specializing in Three.js.

Your task is to generate beautiful, visually interesting 3D shaders based on the user's description.

GEOMETRY:
- ALWAYS use geometryType = ""icosahedron"".
- Never choose another geometry type.
- The geometry type is fixed and must not be changed based on the user's prompt.

SHADER REQUIREMENTS:

1. Generate vertex and fragment shaders for Three.js ShaderMaterial.
2. Use GLSL 1.0 syntax only.
3. Use gl_FragColor in the fragment shader.
4. Use varying to pass data between vertex and fragment shaders.
5. Never use #version 300 es.
6. Never use GLSL 3.0 syntax.
7. Do not use external textures.
8. Do not invent custom uniforms.
9. Use Three.js built-in variables:
   - projectionMatrix
   - modelViewMatrix
   - normalMatrix
   - position
   - normal
10. Do not declare built-in uniforms or attributes manually.
11. Never use uProjectionMatrix, uModelViewMatrix or uNormalMatrix.
12. Calculate vertex position using:
    gl_Position = projectionMatrix * modelViewMatrix * vec4(transformed, 1.0);
13. Vertex and fragment shaders must have matching varying variables.
14. Vertex shader must correctly transform normals using normalMatrix.
15. Make geometry deformation visually interesting when requested.
16. Use lighting, color variation, facets, fresnel and specular effects when appropriate.
17. Avoid excessive metallic reflections unless explicitly requested.
18. Keep shaders compatible with Three.js ShaderMaterial.

OUTPUT FORMAT:

Return ONLY a valid JSON object with exactly these fields:

{
    ""geometryType"": ""icosahedron"",
    ""vertexShaderCode"": ""..."",
    ""fragmentShaderCode"": ""...""
}

The geometryType value MUST always be ""icosahedron"".

Do not use Markdown code blocks.
Do not include explanations outside JSON.

USER REQUEST:
"


                },
                new
                {
                    role = "user",
                    content = prompt
                }
            },
            temperature = 0.3,
            stream = false
        };

        var response = await _httpClient.PostAsJsonAsync(
            "chat/completions",
            request,
            cancellationToken);

        var responseContent = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"LM Studio returned {(int)response.StatusCode}: {responseContent}");
        }

        var chatResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(
            responseContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        var content = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException(
                "LM Studio returned an empty response.");
        }

        var json = ExtractJsonObject(content);

        var result = JsonSerializer.Deserialize<GeneratedShaderResult>(
            json,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        if (result is null ||
            string.IsNullOrWhiteSpace(result.VertexShaderCode) ||
            string.IsNullOrWhiteSpace(result.FragmentShaderCode))
        {
            throw new InvalidOperationException(
                "LM Studio returned an invalid shader response.");
        }

        var allowedGeometryTypes = new[]
        {
            "octahedron",
            "icosahedron",
            "sphere"
        };

        var geometryType = allowedGeometryTypes.FirstOrDefault(
            x => string.Equals(
                x,
                result.GeometryType,
                StringComparison.OrdinalIgnoreCase));

        if (geometryType is null)
        {
            throw new InvalidOperationException(
                $"Unsupported geometry type: {result.GeometryType}");
        }

        result = result with { GeometryType = geometryType };

        return result;
    }

    private static string ExtractJsonObject(string content)
    {
        var start = content.IndexOf('{');
        var end = content.LastIndexOf('}');

        if (start < 0 || end <= start)
        {
            throw new InvalidOperationException(
                "LM Studio did not return a valid JSON object.");
        }

        return content.Substring(start, end - start + 1);
    }

    private sealed class ChatCompletionResponse
    {
        public List<Choice>? Choices { get; set; }
    }

    private sealed class Choice
    {
        public Message? Message { get; set; }
    }

    private sealed class Message
    {
        public string? Content { get; set; }
    }
}