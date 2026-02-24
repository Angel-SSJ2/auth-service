using AuthService.Api.Extensions;
using AuthService.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURACION DE RUTAS
builder.Services.AddControllers();

// CONFIGURACION DE SERVICIOS POR MEDIO DE METODOS DE EXTENSION
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// INICIALIZACION DE LA BASE DE DATOS
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try{
        logger.LogInformation("Iniciando migracion de la base de datos...");
        
        await context.Database.EnsureCreatedAsync(); // Crea la base de datos si no existe

        logger.LogInformation("Migracion completada exitosamente.");
        await DataSeeder.SeedAsync(context); // Seed de datos iniciales
        logger.LogInformation("Seed de datos compleatada exitosamente.");
    } catch(Exception ex){
        logger.LogError(ex, "Error al inicializar la base de datos");
        throw; // Detener la aplicación si la inicialización falla
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
