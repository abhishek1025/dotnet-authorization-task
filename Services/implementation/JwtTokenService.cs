using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace authorization_project.Services.implementation;

public class JwtTokenService: IJwtTokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtTokenService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateToken(string userId, string email)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            Environment.GetEnvironmentVariable("JWT_SECRET_KEY"))
        );
        
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sid,userId),
            new Claim(JwtRegisteredClaimNames.Email,email),
        };
        var token = new JwtSecurityToken(Environment.GetEnvironmentVariable("ISSUER"),
            Environment.GetEnvironmentVariable("ISSUER"),
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GetUserIdFromToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext == null)
        {
            System.Diagnostics.Debug.WriteLine("HttpContext is null in JwtTokenService");
            return null;  // Avoid NullReferenceException
        }
        
        var sidClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c 
            => c.Type == JwtRegisteredClaimNames.Sid);

        
        return sidClaim.Value;
    }
}