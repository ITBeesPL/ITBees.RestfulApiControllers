namespace ITBees.RestfulApiControllers.Models;

public class FasApiErrorVm
{
    public int StatusCode { get; }
    public string ErrorKey { get; }

    public FasApiErrorVm(string message, int statusCode, string errorKey)
    {
        StatusCode = statusCode;
        Message = message ?? string.Empty;
        ErrorKey = errorKey ?? string.Empty;
    }

    public string Message { get; set; }
}