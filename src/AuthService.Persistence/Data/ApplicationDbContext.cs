using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Data{

public class ApplicationDbContext : DbContext
{
    // Metodo Constructor
    // El constructor de la clase ApplicationDbContext recibe un objeto DbContextOptions<ApplicationDbContext> que contiene la configuración necesaria para establecer la conexión a la base de datos y configurar el contexto de Entity Framework Core. Este constructor se utiliza para inicializar el contexto con las opciones proporcionadas.
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Representación de las tablas en el modelo
    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserEmail> UserEmails { get; set; }
    public DbSet<UserPasswordReset> UserPasswordResets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);
    foreach (var entity in modelBuilder.Model.GetEntityTypes())
    {
        var tableName = entity.GetTableName();
        if (!string.IsNullOrEmpty(tableName))
        {
            entity.SetTableName(ToSnakeCase(tableName));
        }
        foreach (var property in entity.GetProperties())
        {
            var columnName = property.GetColumnName();
            if (!string.IsNullOrEmpty(columnName))
            {
                property.SetColumnName(ToSnakeCase(columnName));
            }
        }
     }

    /*
        -------------------------------------------------------------------------------------
        CONFIGURACION ESPECIFICA DE LA ENTIDAD USER
        -------------------------------------------------------------------------------------
    */
    modelBuilder.Entity<User>(entity =>
    {
        // Llave primaria\
        entity.HasKey(e => e.Id);

        // Indices unicos
        entity.HasIndex(e => e.Email).IsUnique();
        entity.HasIndex(e => e.Username).IsUnique();

        // Relacion uno a uno con UserProfile
        entity.HasOne(e => e.UserProfile)
              .WithOne(up => up.user)
              .HasForeignKey<UserProfile>(up => up.UserId)
              .OnDelete(DeleteBehavior.Cascade);
        // Relacion uno a muchos con UserRole
        entity.HasMany(e => e.UserRoles)
              .WithOne(ur => ur.User)
              .HasForeignKey(ur => ur.UserId)
              .OnDelete(DeleteBehavior.Cascade);
        // Relacion uno a uno con UserPasswordReset
        entity.HasOne(e => e.UserPasswordReset)
              .WithOne(ur => ur.User)
              .HasForeignKey<UserPasswordReset>(ur => ur.UserId)
              .OnDelete(DeleteBehavior.Cascade);
        // Relacion uno a uno con UserEmail
        entity.HasOne(e => e.UserEmail)
              .WithOne(ue => ue.User)
              .HasForeignKey<UserEmail>(ue => ue.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // Configuración específica de la entidad UserRole
    modelBuilder.Entity<UserRole>(entity =>
    {
        // Llave primaria compuesta
        entity.HasKey(e => e.Id);

        // Indice unico para evitar duplicados en la asignación de roles a usuarios
        // El usuario no puede tener el mismo rol asignado más de una vez
        entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
    });

    modelBuilder.Entity<Role>(entity =>
    {
        // Llave primaria
        entity.HasKey(e => e.Id);

        // Indice unico para el nombre del rol
        entity.HasIndex(e => e.Name).IsUnique();
    });
    }

    // Funcion para configurar el nombre de clase a nombre de DB
    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
 
        return string.Concat(
            input.Select((x, i) => i > 0 && char.IsUpper(x) 
                ? "_" + x.ToString().ToLower() 
                : x.ToString().ToLower())
        );
    }
}
}