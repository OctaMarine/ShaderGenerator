using UserService.Application;
using UserService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("DefaultConnection")!);
var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();