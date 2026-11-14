using ShaderService.Application;
using ShaderService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!);
var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();