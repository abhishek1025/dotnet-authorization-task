namespace authorization_project.utils.Response;

public class ApiSuccessResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object Data { get; set; }
    public int StatusCode { get; set; }
}