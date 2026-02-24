using System.ComponentModel.DataAnnotations;
namespace AuthService.Domain.Entities;
 
public class Role
{
    [Key]
    [MaxLength(16)]
    public string Id { get; set; } = string.Empty;
 
    [Required(ErrorMessage = "El nombre del rol es requerido")]
    [MaxLength(100,ErrorMessage = "El nombre del rol debe tener menos de 100 caracteres")]
    public string Name { get; set; } = string.Empty;
 
    //Relaciones con UserRole
    public ICollection<UserRole> UserRoles { get; set; }
}
 
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
 