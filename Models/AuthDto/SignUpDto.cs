using System.ComponentModel.DataAnnotations;

namespace authorization_project.Models.User;

public class SignUpDto
{
        public string Name { get; set; }
        public string Email { get; set; }
        public string password { get; set; }
        public bool IsAdmin { get; set; }
}