
namespace authorization_project.Models.User;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string password { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}