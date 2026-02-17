using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IRoleRepository 
{
    Task<Role?> GetByNameAsync(string name);
    Task<int> CountUserInRoleAsync(string roleId);
    Task<IReadOnlyList<User>> GetUserByRoleAsync(string roleId);
    Task<IReadOnlyList<string>> GetUserRoleNameAsync(string userId);
}