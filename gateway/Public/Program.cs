using SmartServices.Framework.Web;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, logging, resilience, service discovery)
builder.AddServiceDefaults();

// Add health checks
builder.Services.AddHealthChecks();

// Configure YARP (API Gateway)
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add service discovery
builder.Services.AddServiceDiscovery();

// Add HTTP clients
builder.Services.AddHttpClient();

var app = builder.Build();

// Map service defaults (health checks, swagger, CORS)
app.MapServiceDefaults();

// Map reverse proxy routes
app.MapReverseProxy();

app.Run();
