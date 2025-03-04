using System.Text;
using authorization_project.Authorization;
using authorization_project.DB;
using authorization_project.Services;
using authorization_project.Services.implementation;
using authorization_project.utils;
using authorization_project.utils.Error;
using authorization_project.utils.Response;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

builder.Services.AddAuthorization(options =>
{
    var resources = Enum.GetValues(typeof(ResourceEnum)).Cast<ResourceEnum>();
    var permissions = Enum.GetValues(typeof(PermissionOperationEnum)).Cast<PermissionOperationEnum>();

    foreach (var resource in resources)
    {
        foreach (var permission in permissions)
        {
            string policyName = $"{resource.ToString()}:{permission.ToString()}";
            
            options.AddPolicy(policyName, policy =>
            {
                policy.Requirements.Add(new PermissionRequirement(resource, permission));
            });
        }
    }
});

builder.Services.AddScoped<IDatabase, Database>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();