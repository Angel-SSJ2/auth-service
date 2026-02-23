using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class RoleRepository(ApplicationDbContext context) : IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await context.Roles
        .Include(r => r.UserRoles) // Se incluye la colección de UserRoles relacionada con el rol
        .FirstOrDefaultAsync(r => r.Name == roleName); // Se busca el rol por su nombre utilizando FirstOrDefaultAsync, lo que devuelve null si no se encuentra ningún rol con ese nombre
    }

    public async Task<int> CountUsersInRoleAsync(string roleName)
    {
        return await context.UserRoles
        .Where(ur => ur.Role.Name == roleName) // Se filtran los UserRoles para contar solo aquellos que están asociados con el rol especificado por su nombre
        .CountAsync(); // Se cuenta el número de UserRoles que cumplen con la condición utilizando
    }

    public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(string roleName)
    {
        return await context.UserRoles
        .Where(ur => ur.Role.Name == roleName) // Se filtran los UserRoles para obtener solo aquellos que están asociados con el rol especificado por su nombre
        .Select(ur => ur.User) // Se seleccionan los usuarios relacionados con los UserRoles filtrados utilizando Select para proyectar solo la propiedad User de cada UserRole
        .Include(u => u.UserProfile) // Se incluye la entidad UserProfile relacionada con cada usuario
        .Include(u => u.UserEmail) // Se incluye la entidad UserEmail relacionada con cada usuario
        .Include(u => u.UserRoles) // Se incluye la colección de UserRoles relacionada con cada usuario
        .ThenInclude(ur => ur.Role) // Se incluye la entidad Role relacionada con cada UserRole utilizando ThenInclude para acceder a la propiedad Role de cada UserRole
        .ToListAsync() // Se convierte el resultado a una lista utilizando ToListAsync, lo que devuelve una lista de usuarios que cumplen con la condición de estar asociados con el rol especificado por su nombre
        .ContinueWith(t => (IReadOnlyList<User>)t.Result); // Se convierte el resultado a IReadOnlyList<User> utilizando ContinueWith para proyectar el resultado de la tarea a un tipo de lista de solo lectura
    }

    public async Task<IReadOnlyList<string>> GetUserRoleNameAsync(string userId)
    {
        return await context.UserRoles
        .Where(ur => ur.UserId == userId) // Se filtran los UserRoles para obtener solo aquellos que están asociados con el usuario especificado por su ID
        .Select(ur => ur.Role.Name) // Se seleccionan los nombres de los roles relacionados con los UserRoles filtrados utilizando Select para proyectar solo la propiedad Name de cada Role relacionada con los UserRoles
        .ToListAsync() // Se convierte el resultado a una lista utilizando ToListAsync, lo que devuelve una lista de nombres de roles que cumplen con la condición de estar asociados con el usuario especificado por su ID
        .ContinueWith(t => (IReadOnlyList<string>)t.Result); // Se convierte el resultado a IReadOnlyList<string> utilizando ContinueWith para proyectar el resultado de la tarea a un tipo de lista de solo lectura
    }
}
