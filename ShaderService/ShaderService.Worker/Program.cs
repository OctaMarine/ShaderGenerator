using ShaderService.Application;
using ShaderService.Application.Abstractions;
using ShaderService.Application.ShaderGenerations;
using ShaderService.Infrastructure;
using ShaderService.Infrastructure.AI;
using ShaderService.Worker;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration
                           .GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException(
                           "Connection string not found.");

builder.Services.AddApplication();
builder.Services.AddScoped<ProcessShaderGenerationService>();

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddHttpClient<IShaderAiClient, LmStudioShaderAiClient>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["LmStudio:BaseUrl"]!);

        client.Timeout = TimeSpan.FromSeconds(
            builder.Configuration.GetValue<int>(
                "LmStudio:TimeoutSeconds"));
    });

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();