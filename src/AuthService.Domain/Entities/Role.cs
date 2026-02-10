using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities;

public class Role
{
    [Key] // Primary key
    [MaxLength(16)] // Assuming a maximum length for the role ID
    public string Id { get; set; }

    [Required] // Name is required
    [MaxLength(50)] // Assuming a maximum length for the role name
    public string Name { get; set; }

    [Required] // Description is required
    [MaxLength(255)] // Assuming a maximum length for the role description
    public string Description { get; set; }

    // Relationships with UserRole
    public ICollection<UserRole> UserRoles { get; set; }
}

// Example of predefined

/*
Roles
+--------------+--------------+------------------+
| Id           | Name         | Description      |
+--------------+--------------+------------------+
| ADMIN        | Admin        | Administrador    |
| USER         | User         | Usuario normal   |
| GUEST        | Guest        | Invitado         |
+--------------+--------------+------------------+
*/
 