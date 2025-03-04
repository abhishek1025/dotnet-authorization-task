using authorization_project.utils;

namespace authorization_project.Services;

public interface IPermissionService
{
    Task<bool> UserHasPermission(string userId, ResourceEnum resource, PermissionOperationEnum operation);
}