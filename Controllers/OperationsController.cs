using authorization_project.DB;
using authorization_project.utils.Response;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace authorization_project.Controllers
{
    [Route("/api/v1/operations")]
    [ApiController]
    [Authorize]
    public class OperationsController : Controller
    {
        private readonly IDatabase _database;

        public OperationsController(IDatabase database)
        {
            _database = database;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllOperations()
        {
            NpgsqlConnection dbConnection = _database.CreateConnection();
            await dbConnection.OpenAsync();

            string query = "SELECT * FROM permissions";

            var data = await dbConnection.QueryAsync(query);
            
            await dbConnection.CloseAsync();

            return RestResponse.Ok(data: data);
        }

    }
}
