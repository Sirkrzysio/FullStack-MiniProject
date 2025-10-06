using System.ComponentModel.DataAnnotations;
using DefaultNamespace.ToDo;

namespace DefaultNamespace;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string PasswordHash { get; set; } = null!;
    
    public string Role { get; set; } = "User";
    
    public ICollection<UploadedFile> Files { get; set; } = new List<UploadedFile>();
}