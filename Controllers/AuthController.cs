using authorization_project.DB;
using authorization_project.Models.User;
using authorization_project.Services;
using authorization_project.utils.Error;
using authorization_project.utils.Response;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using static BCrypt.Net.BCrypt;

namespace authorization_project.Controllers
{
    [Route("/api/v1/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IDatabase _database;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IDatabase database, IJwtTokenService jwtTokenService)
        {
            _database = database;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto newUser)
        {
            var salt = GenerateSalt(12);
            var hashedPassword = HashPassword(newUser.password, salt);

            newUser.password = hashedPassword;

            NpgsqlConnection dbConnection = _database.CreateConnection();
            await dbConnection.OpenAsync();

            string query =
                "INSERT INTO USERS (Name, Email, Password, Is_Admin) VALUES (@Name, @Email, @Password, @IsAdmin)";

            var user = await dbConnection.ExecuteAsync(query, newUser);

            if (user == 0)
            {
                throw new CustomException("Unable to create User.", StatusCodes.Status400BadRequest);
            }

            await dbConnection.CloseAsync();

            return RestResponse.Ok(
                StatusCodes.Status201Created,
              "User Created Successfully."
            );
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto signInBody)
        {
            NpgsqlConnection dbConnection = _database.CreateConnection();
            
            await dbConnection.OpenAsync();

            string query = "SELECT * FROM users where email = @Email";

            User? user = await dbConnection.QueryFirstOrDefaultAsync<User>(query, new { Email = signInBody.Email });

            if (user == null)
            {
                throw new CustomException("User not found", StatusCodes.Status404NotFound);
            }

            bool isPasswordMatched = Verify(signInBody.password, user.password);

            if (!isPasswordMatched)
            {
                throw new CustomException("Password or Email is incorrect", StatusCodes.Status400BadRequest);
            }

            string token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Email);
            
            await dbConnection.CloseAsync();

            return RestResponse.Ok(data:new
            {
                Token = token
            }, message:"Authentication Successfull");
        }
    }
}