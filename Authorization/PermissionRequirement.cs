using authorization_project.utils;
using Microsoft.AspNetCore.Authorization;

namespace authorization_project.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public ResourceEnum Resource { get; }
    public PermissionOperationEnum Operation { get; }

    public PermissionRequirement(ResourceEnum resource, PermissionOperationEnum operation)
    {
        Resource = resource;
        Operation = operation;
    }
}