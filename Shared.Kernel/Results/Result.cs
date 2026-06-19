namespace Shared.Kernel.Results;

public enum ErrorType
{
    Validation = 400,
    Unauthorized = 401,
    NotFound = 404,
    Conflict = 409,
    InternalError = 500
}

public enum SuccessType
{
    Ok = 200,
    Created = 201,
    NoContent = 204
}

public record Success<TValue>(TValue? Value, SuccessType Type);
public record Error(string Message, ErrorType Type, object? Details = null);

public class Result<TValue>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    
    public Success<TValue>? Success { get; }
    public Error? Error { get; }

    public TValue? Value => Success != null ? Success.Value : default;

    private Result(Success<TValue>? success, bool isSuccess, Error? error)
    {
        Success = success;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<TValue> Ok(TValue value) => 
        new(new Success<TValue>(value, SuccessType.Ok), true, null);

    public static Result<TValue> Created(TValue value) => 
        new(new Success<TValue>(value, SuccessType.Created), true, null);

    public static Result<TValue> NoContent() => 
        new(new Success<TValue>(default, SuccessType.NoContent), true, null);
    
    public static Result<TValue> Failure(string message, ErrorType type, object? details = null) => 
        new(
            null,
            false, 
            new Error(message, type, details)
        );
}