using JWTAuthenticationManager;
using ProductAPI.RabbitmqService;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Eureka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddServiceDiscovery(o => o.UseEureka());

builder.Services.AddScoped<IMessageProducer, MessageProducer>();

builder.Services.AddControllers();
// builder.Services.AddCustomJwtAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
