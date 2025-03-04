using authorization_project.DB;
using authorization_project.utils.Response;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace authorization_project.Controllers
{
    [Route("/api/v1/users")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IDatabase _database;

        public UserController(IDatabase database)
        {
            _database = database;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            NpgsqlConnection dbConnection = _database.CreateConnection();
            await dbConnection.OpenAsync();

            string query = "SELECT id, name, email, is_admin FROM users";

            var data = await dbConnection.QueryAsync(query);
            
            await dbConnection.CloseAsync();

            return RestResponse.Ok(data: data);
        }

    }
}
