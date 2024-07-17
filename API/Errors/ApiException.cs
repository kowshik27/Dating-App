namespace API.Controllers;

public class ApiException(int statusCode, string message, string? details)
{
    public int StatusCode { get; set; } = statusCode;
    public string ErrorMessage { get; set; } = message;
    public string? ErrorDetails { get; set; }= details;
}
