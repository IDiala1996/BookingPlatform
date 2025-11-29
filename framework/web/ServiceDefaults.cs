using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Polly;
using SmartServices.Framework.Utilities.Security;
using SmartServices.Framework.Utilities.Communication;
using SmartServices.Framework.Utilities.State;

namespace SmartServices.Framework.Web;

/// <summary>
/// Extension methods for configuring service defaults for microservices.
/// Includes OpenTelemetry, logging, resilience, and service discovery configuration.
/// </summary>
public static class ServiceDefaults
{
    /// <summary>
    /// Adds service defaults to the builder.
    /// Configures OpenTelemetry, logging, HTTP clients, and service discovery.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The modified builder.</returns>
    public static WebApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        var serviceName = builder.Environment.ApplicationName;



        // Add OpenTelemetry
        var otlpExporterEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] ?? "http://localhost:4317";

        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName));
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;

            // Export logs using OTLP (to collector or backend)
            options.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Endpoint = new Uri(otlpExporterEndpoint); // Replace with your collector endpoint
            });
        });


        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(
                    serviceName: serviceName,
                    serviceVersion: typeof(ServiceDefaults).Assembly.GetName().Version?.ToString() ?? "1.0.0"
                ))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation(options =>
                {
                    // Exclude health check endpoints from tracing
                    options.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health");
                })
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpExporterEndpoint);
                    options.Protocol = OtlpExportProtocol.Grpc;
                })
            )
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpExporterEndpoint);
                    options.Protocol = OtlpExportProtocol.Grpc;
                })
            );

        // Configure HTTP clients with resilience
        builder.Services.AddHttpClient()
            .ConfigureHttpClientDefaults(http =>
            {
                http.AddStandardResilienceHandler();
            });

        // Add service discovery
        builder.Services.AddServiceDiscovery();

        // Add CORS for microfrontends
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowMicrofrontends", corsPolicyBuilder =>
            {
                corsPolicyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });


        // Register Dapr utilities
        builder.Services.AddDaprClient();
        builder.Services.AddScoped<ICryptographyService, DaprCryptographyService>();
        builder.Services.AddScoped<IServiceInvocationClient, DaprServiceInvocationClient>();
        builder.Services.AddScoped<ICacheService, DaprCacheService>();

        // Add OpenAPI/Swagger
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();

        return builder;
    }

    /// <summary>
    /// Maps default service endpoints including health checks and Swagger.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The modified application.</returns>
    public static WebApplication MapServiceDefaults(this WebApplication app)
    {
        // Enable CORS
        app.UseCors("AllowMicrofrontends");
        // Enable Dapr subscriptions
        app.MapSubscribeHandler();

        // Health checks endpoint (excluded from OpenTelemetry)
        app.MapHealthChecks("/health");
        app.MapHealthChecks("/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
        {
            Predicate = reg => reg.Tags.Contains("ready"),
        });

        // Swagger/OpenAPI
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}

/// <summary>
/// Extension methods for configuring HTTP client resilience.
/// </summary>
internal static class ResilienceExtensions
{
    /// <summary>
    /// Adds standard resilience policies to HTTP clients (retry, circuit breaker, timeout).
    /// </summary>
    /// <param name="builder">The HTTP client builder.</param>
    /// <returns>The modified builder.</returns>
    public static IHttpClientBuilder AddStandardResilienceHandler(this IHttpClientBuilder builder)
    {
        return builder
            .AddTransientHttpErrorPolicy(policy =>
                policy.WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(Math.Pow(2, attempt) * 100),
                    onRetry: (outcome, timespan, retryCount, context) =>
                    {
                        // Logging handled by middleware
                    }
                )
            )
            .AddTransientHttpErrorPolicy(policy =>
                policy.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)
                )
            );
    }
}
