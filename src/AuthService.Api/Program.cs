using AuthService.Api.Extensions;
using AuthService.Api.Middlewares;
using AuthService.Api.ModelBinders;
using AuthService.Persistence.Data;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using Serilog;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// FIX: Bypass SSL (Solo para desarrollo)
System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new FileDataModelBinderProvider());
})
.AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

// CONFIGURACIÓN DE SERVICIOS
builder.Services.AddApiDocumentation();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRateLimitingPolicies();
builder.Services.AddSecurityPolicies(builder.Configuration);
builder.Services.AddSecurityOptions();

// --- 1. REGISTRO DE POLÍTICA CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// --- CONFIGURACIÓN DEL PIPELINE ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

// --- 2. CONFIGURACIÓN DE SECURITY HEADERS (CORREGIDO) ---
app.UseSecurityHeaders(policies => policies
    .AddDefaultSecurityHeaders()
    .RemoveServerHeader()
    .AddFrameOptionsDeny()
    .AddXssProtectionBlock()
    .AddContentTypeOptionsNoSniff()
    .AddReferrerPolicyStrictOriginWhenCrossOrigin()
    .AddContentSecurityPolicy(cspBuilder =>
    {
        cspBuilder.AddDefaultSrc().Self();
        cspBuilder.AddScriptSrc().Self().UnsafeInline();
        cspBuilder.AddStyleSrc().Self().UnsafeInline();
        cspBuilder.AddImgSrc().Self().Data();
        cspBuilder.AddFontSrc().Self().Data();
        
        // FIX DEFINITIVO: Usamos un solo string con los orígenes separados por espacios
        // Esto es lo que dicta el estándar de CSP y lo que la librería acepta sin errores de sobrecarga
        cspBuilder.AddConnectSrc()
            .Self()
            .From("http://localhost:5212 https://localhost:5212 http://localhost:5173 ws://localhost:5173");
            
        cspBuilder.AddFrameAncestors().None();
        cspBuilder.AddBaseUri().Self();
        cspBuilder.AddFormAction().Self();
    })
    .AddCustomHeader("Permissions-Policy", "geolocation=(), microphone=(), camera=()")
    .AddCustomHeader("Cache-Control", "no-store, no-cache, must-revalidate, private")
);

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

// --- 3. CORS ---
app.UseCors("DefaultCorsPolicy"); 

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// HEALTH CHECKS
app.MapHealthChecks("/health");
app.MapGet("/health", () =>
{
    var response = new { status = "Healthy", timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") };
    return Results.Ok(response);
});
app.MapHealthChecks("/api/v1/health");

// STARTUP LOG
var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        var server = app.Services.GetRequiredService<IServer>();
        var addressesFeature = server.Features.Get<IServerAddressesFeature>();
        var addresses = (IEnumerable<string>?)addressesFeature?.Addresses ?? app.Urls;
        if (addresses != null && addresses.Any())
        {
            foreach (var addr in addresses)
            {
                var health = $"{addr.TrimEnd('/')}/health";
                startupLogger.LogInformation("AuthService API is running at {Url}. Health endpoint: {HealthUrl}", addr, health);
            }
        }
    }
    catch (Exception ex)
    {
        startupLogger.LogWarning(ex, "Failed to determine the listening addresses");
    }
});

// INITIALIZE DATABASE
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Checking database connection...");
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Database ready. Running seed data...");
        await DataSeeder.SeedAsync(context);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed.");
        throw; 
    }
}

app.Run();