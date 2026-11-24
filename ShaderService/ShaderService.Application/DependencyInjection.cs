using Microsoft.Extensions.DependencyInjection;
using ShaderService.Application.ShaderGenerations;


namespace ShaderService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ShaderGeneratorService>();
        services.AddScoped<ProcessShaderGenerationService>();
        services.AddScoped<GetShaderGenerationService>();

        return services;
    }
}