using CartAPI.RabbitmqService;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ReceiveService>();

builder.Services.AddServiceDiscovery(o => o.UseEureka());

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();
