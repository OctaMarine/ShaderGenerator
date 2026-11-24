using ShaderService.Application;
using ShaderService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!);
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("Client");
app.MapControllers();

app.Run();