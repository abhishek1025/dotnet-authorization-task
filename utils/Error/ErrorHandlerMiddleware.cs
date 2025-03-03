using System.Net;
using authorization_project.utils.Response;
using Npgsql;

namespace authorization_project.utils.Error;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    
    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(
                new ApiErrorResponse(){ Success = false, Message = "Unauthorized access." });
        }
            
        catch (Exception exception)
        {
            int statusCode = StatusCodes.Status500InternalServerError;
            string message = exception.Message ?? "An unexpected error occurred";
            
            if (exception is CustomException apiEx) 
            {
                statusCode = apiEx.StatusCode;
            }
            
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(
                new ApiErrorResponse() { Success = false, Message = message, StatusCode = statusCode});
        }
        
    }
}