using AuthService.Domain.Entities;
namespace AuthService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user);
    Task<User> GetByIdAsync(string id);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByEmailVerificationTokenAsync(string token);
    Task<User> GetByPasswordResetTokenAsync(string token);
    Task<User> ExistByEmailAsync(string email);
    Task<User> ExistByUsernameAsync(string username);
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(string id);
    Task UpdatePasswordAsync(string userId, string newPassword);



}