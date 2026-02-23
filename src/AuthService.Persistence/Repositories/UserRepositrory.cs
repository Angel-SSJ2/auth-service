using AuthService.Application.Service;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(string id)
    {
        var user = await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRole)
            .FirstOrDefaultAsync(u => u.Id == id);

        return user ?? throw new InvalidOperationException($"User with ID '{id}' not found.");
    }
//3
    public async Task<User> CreateAsync(string email)
    {
       return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRole)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => EF.Functions.ILike(u.Email, email));
    }
//4
    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRole)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => EF.Functions.Like(u.Username, username));
    }
//5
    public async Task<User> GetByEmailVerificationTokenAsync(string token)
    {
        var user = await context.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserEmail)
            .Include(u => u.UserPasswordReset)
            .Include(u => u.UserRole)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserEmail !=null && u.UserEmail.EmailVerificationToken == token);

        return user ?? throw new InvalidOperationException($"User with email verification token '{token}' not found.");
    }
//6 
    public async Task<User> CreateAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }
//7
    public async Task<User> UpdateAsync(User user)
    {
        await context.SaveChangesAsync();
        return await GetByIdAsync(user.Id);
    }
//8
    public async Task<bool> DeleteAsync(string id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            return false;
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }
//9
    public async Task<Boolean> ExistByEmailAsync(string email)
    {
        return await context.Users
        .AnyAsync(u => EF.Functions.ILike(u.Email, email));
    }
//10
    public async Task<bool> ExistByUsernameAsync(string username)
    {
        return await context.Users
        .AnyAsync(u => EF.Functions.ILike(u.Username, username));
    }
//11 
    public async Task UpdatePasswordAsync(string userId, string roleId)
    {
        var existingRole = await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .ToListAsync();

        context.UserRoles.RemoveRange(existingRole);

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