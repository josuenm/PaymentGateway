using Microsoft.AspNetCore.Mvc;

namespace Shared.Kernel.Results;

public record ErrorResult(
    string Message,
    Dictionary<string, IEnumerable<string>>? Details = null,
    string? TraceId = null
);

public record ResultObject<TData>(
    TData? Data,
    bool Success, 
    ErrorResult? Error
);

public class Result<TData>
{
    public int StatusCode;
    public bool Success;
    public TData Data;
    public ErrorResult? Error = null;
    
    private Result() { }

    public static Result<TData> Ok(TData data) => new()
    {
        Data = data,
        Success = true,
        StatusCode = 200
    };

    public static Result<TData> Created(TData data) => new()
    {
        Data = data,
        Success = true,
        StatusCode = 201
    };

    public static Result<TData> NotFound(string message) => new()
    {
        Success = false,
        StatusCode = 404,
        Error = new ErrorResult(message)
    };
    
    public static Result<TData> BadRequest(string message) => new()
    {
        Success = false,
        StatusCode = 400,
        Error = new ErrorResult(message)
    };

    public static Result<TData> Unauthorized(string message) => new()
    {
        Success = false,
        StatusCode = 401,
        Error = new ErrorResult(message)
    };

    public static Result<TData> Conflict(string message) => new()
    {
        Success = false,
        StatusCode = 409,
        Error = new ErrorResult(message)
    };
    
    public static Result<TData> InternalServerError(string message) => new()
    {
        Success = false,
        StatusCode = 500,
        Error = new ErrorResult(message)
    };

    public object ToObject()
    {
        return new ResultObject<TData>(Data, Success, Error);
    }
    
    public IActionResult ToActionResult()
    {
        return new ObjectResult(new ResultObject<TData>(Data, Success, Error))
        {
            StatusCode = StatusCode
        };
    }
}