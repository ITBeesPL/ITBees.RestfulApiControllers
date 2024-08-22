using Microsoft.AspNetCore.Http;

namespace ITBees.RestfulApiControllers.Models;

public class FasApiErrorVm
{
    public int StatusCode { get; }
    public string ErrorKey { get; }

    /// <summary>
    /// Create instance of FasApiErrorVm, which will be used to generate error respone, based on provided status code
    /// </summary>
    /// <param name="message"></param>
    /// <param name="statusCode">Please provide correct status code for example use static class code like : StatusCodes.Status403Forbidden</param>
    /// <param name="errorKey"></param>
    public FasApiErrorVm(string message, int statusCode, string errorKey)
    {
        StatusCode = statusCode;
        Message = message ?? string.Empty;
        ErrorKey = errorKey ?? string.Empty;
    }

    public string Message { get; set; }
}