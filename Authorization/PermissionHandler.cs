using System.Security.Claims;
using authorization_project.Services;
using Microsoft.AspNetCore.Authorization;

namespace authorization_project.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly IJwtTokenService _jwtTokenService;
    
    public PermissionHandler(IPermissionService permissionService, IJwtTokenService jwtTokenService)
    {
        _permissionService = permissionService;
        _jwtTokenService = jwtTokenService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        PermissionRequirement requirement)
    {
        var userId = _jwtTokenService.GetUserIdFromToken();
        
        if (string.IsNullOrEmpty(userId))
        {
            return; 
        }

        bool hasPermission = await _permissionService.UserHasPermission(userId, requirement.Resource, requirement.Operation);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        
    }
    
}