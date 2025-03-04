using System.ComponentModel.DataAnnotations.Schema;

namespace authorization_project.Models.User.Permissions;

public class UserPermission
{
    public Guid Id { set; get; }
    
    [Column("user_id")]
    public Guid UserId { set; get; }
    
    public string Resource { set; get; }
    
    [Column("permission_id")]
    public int PermissionId { set; get; }
    
    [Column("created_on")]
    public DateTime CreatedOn { get; set; }
}