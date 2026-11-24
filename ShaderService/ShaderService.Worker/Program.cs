using ShaderService.Application;
using ShaderService.Infrastructure;
using ShaderService.Worker;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration
                           .GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException(
                           "Connection string not found.");

builder.Services.AddApplication();

builder.Services.AddInfrastructure(connectionString);

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

await host.RunAsync();