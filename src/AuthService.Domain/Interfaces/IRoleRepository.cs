using AuthService.Domain.Entities;

namespace AuthService.Domain.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string name);
    Task<int> CountUsersInRoleAsync(string roleId);
    Task<IReadOnlyList<Role>> GetUsersByRoleAsync(string roleId);
    Task<IReadOnlyList<string>> GetUserRoleNameAsync(string userId);
}