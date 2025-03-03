namespace authorization_project.Services;

public interface IJwtTokenService
{
    public string GenerateToken(string userId, string email);
    public string GetUserIdFromToken();
}