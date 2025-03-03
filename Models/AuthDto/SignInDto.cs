using System.ComponentModel.DataAnnotations;

namespace authorization_project.Models.User;

public class SignInDto
{
    public string Email { get; set; }
    public string password { get; set; }
 
}