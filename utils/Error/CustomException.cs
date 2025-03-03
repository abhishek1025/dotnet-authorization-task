namespace authorization_project.utils.Error;

public class CustomException : Exception
{
    public int StatusCode { get; set; } 
    
    public CustomException( string message,int statusCode = StatusCodes.Status500InternalServerError, Exception innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}