using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShaderService.Application.Abstractions;

namespace ShaderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IShaderRepository, ShaderRepository>();

        return services;
    }
}