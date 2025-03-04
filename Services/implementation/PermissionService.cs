using authorization_project.DB;
using authorization_project.utils;
using Dapper;
using Npgsql;

namespace authorization_project.Services.implementation;

public class PermissionService : IPermissionService
{
    private readonly IDatabase _database;

    public PermissionService(IDatabase database)
    {
        _database = database;
    }

    public async Task<bool> UserHasPermission(string userId, ResourceEnum resource, PermissionOperationEnum operation)
    {
        NpgsqlConnection dbConnection = _database.CreateConnection();
        
        await dbConnection.OpenAsync();

        string query = @"SELECT COUNT(*) FROM user_permission UP INNER
            JOIN permissions P ON UP.permission_id = P.id
            WHERE up.user_id = @userId AND UP.resource = @resource AND P.operation = @operation";

        var count = await dbConnection.ExecuteScalarAsync<int>(query, new { userId = Guid.Parse(userId), resource = resource.ToString(), operation = operation.ToString() });
        
        await dbConnection.CloseAsync();
        
        return count > 0;
    }
}