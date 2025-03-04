using System.ComponentModel.DataAnnotations;

namespace authorization_project.Models.User.Permissions;

public class CreatePermissionDto
{
    [Required]
    public int permissionId { get; set; }
    [Required]
    public string resource { get; set; }
    [Required]
    public Guid userId { get; set; }
}