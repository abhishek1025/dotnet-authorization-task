using System.Text;
using authorization_project.DB;
using authorization_project.Services;
using authorization_project.Services.implementation;
using authorization_project.utils.Error;
using authorization_project.utils.Response;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

// Authentication
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme
).AddJwtBearer(
    x =>
    {
      
        x.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("ISSUER"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET_KEY")))
        };
        x.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();

                    logger.LogWarning("No token provided in the Authorization header.");

                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new ApiErrorResponse
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Message = "Invalid or expired token. Please login again."
                    };

                    return context.Response.WriteAsJsonAsync(response);
                }

                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerHandler>>();

                logger.LogError("Authentication failed: {Message}", context.Exception.Message);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                
                var response = new ApiErrorResponse()
                {
                    Success = false,
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Invalid or expired token. Please login again."
                };

                return context.Response.WriteAsJsonAsync(response);
            }
        };
    }
);


builder.Services.AddScoped<IDatabase, Database>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

Env.Load();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();