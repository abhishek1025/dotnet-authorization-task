using Microsoft.AspNetCore.Mvc;

namespace authorization_project.utils.Response;

public class RestResponse
{
    public static IActionResult Ok( int statusCode = StatusCodes.Status200OK, string message = "SUCCESS", object data = null)
    {
        ApiSuccessResponse response = new()
        {
            Data = data,
            StatusCode = statusCode,
            Message = message,
            Success = true
        };
     
        return new JsonResult(response)
        {
            StatusCode = statusCode
        };
    }


    public static IActionResult Error(int statusCode, string message = "SUCCESS")
    {
        ApiErrorResponse response = new ()
        {
            Success = false,
            Message = message,
            StatusCode = statusCode
        };
      
        return new JsonResult(response)
        {
            StatusCode = statusCode
        };
    }
}
