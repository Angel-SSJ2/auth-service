using AuthService.Domain.Entities;
using AuthService.Domain.Constants;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
    IConfiguration configuration) 
    {
        // Se registra el ApplicationDbContext en el contenedor de servicios utilizando la cadena de conexión definida en el archivo de configuración (appsettings.json) bajo la clave "DefaultConnection". Se utiliza el proveedor de base de datos Npgsql para PostgreSQL y se configura para usar la convención de nomenclatura en snake_case.
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
            .UseSnakeCaseNamingConvention());
        
        // Se registra el servicio de inicialización de datos (DataSeeder) en el contenedor de servicios con un alcance transitorio (Transient), lo que significa que se creará una nueva instancia del servicio cada vez que se solicite.
        services.AddHealthChecks();

        return services;
}
}
