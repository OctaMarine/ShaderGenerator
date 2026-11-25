
using ShaderService.Application.Abstractions;
using ShaderService.Domain.Entities;
using ShaderService.Domain.Enums;

namespace ShaderService.Application.ShaderGenerations;

public class ProcessShaderGenerationService
{
    private readonly IShaderRepository _shaderRepository;
    private readonly IShaderAiClient _shaderAiClient;


    public ProcessShaderGenerationService(
        IShaderRepository shaderRepository,
        IShaderAiClient shaderAiClient)
    {
        _shaderRepository = shaderRepository;
        _shaderAiClient = shaderAiClient;

    }

    public async Task ProcessAsync(
        Guid generationId,
        CancellationToken cancellationToken)
    {
        var generation = await _shaderRepository.GetByIdAsync(
            generationId,
            cancellationToken);

        if (generation is null)
        {
            throw new InvalidOperationException(
                $"Shader generation {generationId} was not found.");
        }

        if (generation.Status == ShaderGenerationStatus.Completed)
        {
            return;
        }

        generation.Status = ShaderGenerationStatus.Processing;

        await _shaderRepository.UpdateAsync(
            generation,
            cancellationToken);

        try
        {
            var result = await _shaderAiClient.GenerateAsync(
                generation.Prompt!,
                cancellationToken);

            generation.GeometryType = result.GeometryType;
            generation.VertexShaderCode = result.VertexShaderCode;
            generation.FragmentShaderCode = result.FragmentShaderCode;

            generation.Status = ShaderGenerationStatus.Completed;

            await _shaderRepository.UpdateAsync(
                generation,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            generation.Status = ShaderGenerationStatus.Failed;

            await _shaderRepository.UpdateAsync(
                generation,
                CancellationToken.None);

            throw;
        }
    }

    private static string GenerateVertexShaderCode()
    {
        return @"
        precision highp float;

        varying vec3 vNormal;
        varying vec3 vViewPosition;
        varying vec3 vObjectPosition;

        void main()
        {
            vec3 direction = normalize(position);
            vec3 transformed = position;

            float broadDistortion =
                sin(direction.x * 4.7 + direction.z * 3.1 + direction.y * 2.2)
                * 0.09;

            float fineDistortion =
                sin(direction.x * 9.0 - direction.y * 5.0 + direction.z * 7.0)
                * 0.055;

            float radialScale = 1.0 + broadDistortion + fineDistortion;

            transformed *= radialScale;

            transformed.x *= 0.88;
            transformed.y *= 1.35;
            transformed.z *= 0.82;

            float upperSpike = pow(
                max(dot(direction, normalize(vec3(0.25, 0.92, 0.1))), 0.0),
                10.0
            );

            float sideSpike = pow(
                max(dot(direction, normalize(vec3(-0.8, 0.25, 0.45))), 0.0),
                12.0
            );

            float lowerSpike = pow(
                max(dot(direction, normalize(vec3(-0.15, -0.95, -0.2))), 0.0),
                9.0
            );

            transformed += direction * (
                upperSpike * 0.22 +
                sideSpike * 0.16 +
                lowerSpike * 0.18
            );

            transformed.x += direction.y * 0.09;
            transformed.z += direction.x * 0.06;

            transformed.y +=
                pow(max(direction.y, 0.0), 4.0) * 0.12;

            transformed.y -=
                pow(max(-direction.y, 0.0), 4.0) * 0.08;

            vec3 transformedNormal = normalize(vec3(
                normal.x / 0.88,
                normal.y / 1.35,
                normal.z / 0.82
            ));

            vec4 mvPosition =
                modelViewMatrix * vec4(transformed, 1.0);

            vNormal = normalize(
                normalMatrix * transformedNormal
            );

            vViewPosition = -mvPosition.xyz;
            vObjectPosition = transformed;

            gl_Position = projectionMatrix * mvPosition;
        }
        ";
    }
    
    private static string GenerateFragmentShaderCode()
    {
        return @"
        precision highp float;
    
        varying vec3 vNormal;
        varying vec3 vViewPosition;
        varying vec3 vObjectPosition;
    
        void main()
        {
            vec3 normal = normalize(vNormal);
            vec3 viewDir = normalize(vViewPosition);
    
            vec3 light1 = normalize(vec3(-0.7, 0.9, 0.8));
            vec3 light2 = normalize(vec3(0.8, 0.3, -0.6));
    
            float diffuse1 = max(dot(normal, light1), 0.0);
            float diffuse2 = max(dot(normal, light2), 0.0);
    
            vec3 deepPurple = vec3(0.045, 0.008, 0.11);
            vec3 darkPurple = vec3(0.12, 0.025, 0.27);
            vec3 purple = vec3(0.28, 0.055, 0.55);
            vec3 violet = vec3(0.48, 0.14, 0.82);
            vec3 lightViolet = vec3(0.65, 0.32, 0.95);
    
            float lighting = diffuse1 * 0.7 + diffuse2 * 0.3;
    
            vec3 color = mix(
                deepPurple,
                darkPurple,
                0.4 + lighting * 0.6
            );
    
            color = mix(
                color,
                purple,
                diffuse1 * 0.65
            );
    
            color = mix(
                color,
                violet,
                diffuse2 * 0.4
            );
    
            float facetPattern = dot(
                normal,
                normalize(vec3(0.35, 0.8, -0.45))
            );
    
            float facetBrightness = 0.5 + facetPattern * 0.5;
    
            color *= 0.8 + facetBrightness * 0.4;
    
            float crystalStriation = sin(
                vObjectPosition.y * 24.0 +
                vObjectPosition.x * 11.0 +
                vObjectPosition.z * 13.0
            );
    
            color += violet * crystalStriation * 0.025;
    
            float specular = pow(
                max(
                    dot(reflect(-light1, normal), viewDir),
                    0.0
                ),
                48.0
            );
    
            color += lightViolet * specular * 0.2;
    
            float fresnel = pow(
                1.0 - max(dot(normal, viewDir), 0.0),
                3.0
            );
    
            color += vec3(0.35, 0.08, 0.65) * fresnel * 0.3;
    
            gl_FragColor = vec4(color, 1.0);
        }
        ";
    }

}
