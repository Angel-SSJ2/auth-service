using AuthService.Domain.Entities;
namespace AuthService.Domain.Entities; 
public interface IUserRepository 
{
    Task<User?> GetByIdAsync(string id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUserNameAsync(string username);
    Task<User?> GetByPasswordResetTokenAsync(string token); 
    Task<User> CreateAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User> DeleteAsync(User user);

    Task<bool> ExistByEmailAsync(string email);
    Task<bool> ExistByUserNameAsync(string username); 

}
