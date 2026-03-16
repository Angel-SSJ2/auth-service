using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    // Se utiliza el método GetByIdAsync para obtener un usuario por su ID, incluyendo sus entidades relacionadas (UserProfile, UserEmail, UserPasswordReset, UserRoles) utilizando la función Include de Entity Framework Core
    public async Task<User?> GetByIdAsync(string id)
    {
        // Se utiliza Include para cargar las entidades relacionadas (UserProfile, UserEmail, UserPasswordReset, UserRoles) junto con el usuario principal
        var user = await context.Users
        .Include(u => u.UserProfile) // Se incluye la entidad UserProfile relacionada con el usuario
        .Include(u => u.UserEmail) // Se incluye la entidad UserEmail relacionada con el usuario
        .Include(u => u.UserPasswordReset) // Se incluye la entidad UserPasswordReset relacionada con el usuario
        .Include(u => u.UserRoles) // Se incluye la colección de UserRoles relacionada con el usuario
        .FirstOrDefaultAsync(u => u.Id == id); // Se busca el usuario por su ID utilizando FirstOrDefaultAsync, lo que devuelve null si no se encuentra ningún usuario con ese ID

        return user;
    }

    // 2. GetByEmailAsync: Este método se utiliza para obtener un usuario por su correo electrónico, incluyendo sus entidades relacionadas (UserProfile, UserEmail, UserPasswordReset, UserRoles) utilizando la función Include de Entity Framework Core. Se utiliza EF.Functions.ILike para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas al buscar el usuario por su correo electrónico. 
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await context.Users
        .Include(u => u.UserProfile)
        .Include(u => u.UserEmail) 
        .Include(u => u.UserPasswordReset) 
        .Include(u => u.UserRoles) 
        .ThenInclude(ur => ur.Role) 
        .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email)); // Se busca el usuario por su correo electrónico utilizando EF.Functions.Like para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas
    }

    // 3. GetByUsernameAsync: Este método se utiliza para obtener un usuario por su nombre de usuario, incluyendo sus entidades relacionadas (UserProfile, UserEmail, UserPasswordReset, UserRoles) utilizando la función Include de Entity Framework Core. Se utiliza EF.Functions.ILike para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas al buscar el usuario por su nombre de usuario.
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
        .Include(u => u.UserProfile) 
        .Include(u => u.UserEmail) 
        .Include(u => u.UserPasswordReset) 
        .Include(u => u.UserRoles) 
        .ThenInclude(ur => ur.Role) 
        .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Username, username)); // Se busca el usuario por su nombre de usuario utilizando EF.Functions.Like para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas
    }

    // 4. GetByEmailVerificationTokenAsync: Este método se utiliza para obtener un usuario por su token de verificación de correo electrónico, incluyendo sus entidades relacionadas (UserProfile, UserEmail, UserPasswordReset, UserRoles) utilizando la función Include de Entity Framework Core. Se busca el usuario por su token de verificación de correo electrónico utilizando una condición que verifica que el token coincida y que la propiedad UserEmail no sea nula para evitar posibles errores de referencia nula.
    public async Task<User?> GetByEmailVerificationTokenAsync(string token)
    {
        return await context.Users
        .Include(u => u.UserProfile) 
        .Include(u => u.UserEmail) 
        .Include(u => u.UserPasswordReset) 
        .Include(u => u.UserRoles) 
        .ThenInclude(ur => ur.Role) 
        .FirstOrDefaultAsync(u => u.UserEmail != null && 
                            u.UserEmail.EmailVerificationToken == token); // Se busca el usuario por su token de verificación de correo electrónico utilizando una condición que verifica que el token coincida y que la propiedad UserEmail no sea nula para evitar posibles errores de referencia nula
    }

    // 5. Busca un usuario mediante su token de restablecimiento de contraseña
    public async Task<User?> GetByPasswordResetTokenAsync(string token)
    {
        return await context.Users
        .Include(u => u.UserProfile)
        .Include(u => u.UserEmail)
        .Include(u => u.UserPasswordReset)
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.UserPasswordReset != null &&
                                  u.UserPasswordReset.PasswordResetToken == token);
    }
 
    // 6. Crea un nuevo registro de usuario en la BD y lo retorna con sus relaciones
    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }
 
    // 7. Actualiza la información de un usuario existente
    public async Task<User> UpdateAsync(User user)
    {
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }
 
    // 8. Elimina un usuario de la base de datos por su ID
    public async Task<bool> DeleteAsync(string id)
    {
        var user = await GetByIdAsync(id);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    // 9. Verifica si existe un usuario con el correo electrónico proporcionado utilizando EF.Functions.ILike para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await context.Users
        .AnyAsync(u => EF.Functions.ILike(u.Email, email)); // Se verifica si existe un usuario con el correo electrónico proporcionado utilizando EF.Functions.Like para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas
    }

    // 10. Verifica si existe un usuario con el nombre de usuario proporcionado utilizando EF.Functions.ILike para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas
    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await context.Users
        .AnyAsync(u => EF.Functions.ILike(u.Username, username)); // Se verifica si existe un usuario con el nombre de usuario proporcionado utilizando EF.Functions.Like para realizar una comparación de cadenas que no distingue entre mayúsculas y minúsculas
    }

    // 11. Actualiza el rol de un usuario eliminando los roles existentes y agregando el nuevo rol proporcionado
    public async Task UpdateUserRoleAsync(string userId, string roleId)
    {
        var existingRoles = await context.UserRoles
        .Where(ur => ur.UserId == userId)
        .ToListAsync();

        context.UserRoles.RemoveRange(existingRoles);

        var newUserRole = new UserRole
        {
            Id = UuidGenerator.GenerateShortUUID(),
            UserId = userId,
            RoleId = roleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        context.UserRoles.Add(newUserRole);
        await context.SaveChangesAsync();
    }
}