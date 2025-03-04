
using System.ComponentModel.DataAnnotations.Schema;

namespace authorization_project.Models.User;

public class User
{
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    public string Email { get; set; }
    public string password { get; set; }
    
    [Column("is_admin")]
    public bool IsAdmin { get; set; }
    
    [Column("created_on")]
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}