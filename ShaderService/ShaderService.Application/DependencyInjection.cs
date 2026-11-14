using Microsoft.Extensions.DependencyInjection;


namespace ShaderService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        //services.AddScoped<ShaderGeneratorService>();

        return services;
    }
}