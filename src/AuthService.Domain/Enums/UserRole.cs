using System.ComponentModel.DataAnnotations;
using AuthService.Domain.Entities;
// space to define the UserRole enumeration, which represents different roles that users can have in the authentication service. This can be used to manage permissions and access control based on the user's role.
namespace AuthService.Domain.Enums;
 
// Enumeration to define user roles in the authentication service
public class UserRole
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;
    [Key]
    [MaxLength(16)]
    public string UserId { get; set; } = string.Empty;
    
    [Key]
    [MaxLength(16)]
    public string RoleId { get; set; } = string.Empty;

    [Required]
    public User User { get; set; } = null!;

    [Required]
    public Role Role { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}