using authorization_project.DB;
using authorization_project.Models.User.Permissions;
using authorization_project.Services;
using authorization_project.utils.Error;
using authorization_project.utils.Response;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace authorization_project.Controllers
{
    [Route("/api/v1/permissions")]
    [ApiController]
    public class PermissionController : Controller
    {
        private readonly IDatabase _database;

        public PermissionController(IDatabase database)
        {
            _database = database;
        }

        [Authorize("PERMISSION:CREATE")]
        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody]CreatePermissionDto createPermissionDto)
        {
            
            NpgsqlConnection dbConnection = _database.CreateConnection();

            await dbConnection.OpenAsync();

            string query = @"INSERT INTO user_permission (permission_id, resource, user_id)
                            VALUES (@PermissionId, @Resource, @UserId)";

            var permission = await dbConnection.ExecuteAsync(query, createPermissionDto);
            
            await dbConnection.CloseAsync();

            if (permission == 0)
            {
                throw new CustomException(
                    message: "Unable to give permission at the moment, please try again later",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            return RestResponse.Ok(
                message: "New permission created successfully",
                statusCode: StatusCodes.Status201Created
            );
        }
        
        [Authorize("PERMISSION:DELETE")]
        [HttpDelete("delete/{user_id}/{resource}/{permission_id}")]
        public async Task<IActionResult> DeletePermission(Guid user_id, string resource, int permission_id)
        {
            NpgsqlConnection dbConnection = _database.CreateConnection();

            await dbConnection.OpenAsync();

            string query = @"DELETE FROM USER_PERMISSION
                            WHERE user_id = @user_id 
                              AND resource = @resource 
                              AND permission_id = @permission_id";

            var permission = await dbConnection.ExecuteAsync(query, new { user_id, resource, permission_id });
            
            await dbConnection.CloseAsync();

            if (permission == 0)
            {
                throw new CustomException(
                    message: "Unable to delete permission at the moment, please try again later",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            return RestResponse.Ok(
                message: "Permission deleted successfully",
                statusCode: StatusCodes.Status201Created
            );
        }

        [Authorize("PERMISSION:READ")]
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions(Guid userId)
        {
            NpgsqlConnection dbConnection = _database.CreateConnection();

            await dbConnection.OpenAsync();

            string query = @"SELECT up.user_id, u.name, u.email, up.resource, p.id as permission_id, p.operation
                            FROM user_permission up 
                            INNER JOIN users u 
                            ON u.id = up.user_id 
                            INNER JOIN PERMISSIONS P 
                            ON p.id = UP.permission_id";
            
            var data = await dbConnection.QueryAsync(query);
            await dbConnection.CloseAsync();
            
            var result = data
                .GroupBy(row => new { row.user_id, row.name, row.email })
                .Select(group => new
                {
                    id = group.Key.user_id,
                    name = group.Key.name,
                    email = group.Key.email,
                    permissions = group
                        .GroupBy(p => p.resource)
                        .Select(resGroup => new
                        {
                            resource = resGroup.Key,
                            operations = resGroup.Select(op => new 
                            { 
                                operation = op.operation,
                                id = op.permission_id 
                            }).ToList()
                        }).ToList()
                }).ToList();

            return RestResponse.Ok(data: result);
        }
    }
}
