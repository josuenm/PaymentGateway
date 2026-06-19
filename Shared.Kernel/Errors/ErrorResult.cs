namespace Shared.Kernel.Errors;

public class ErrorResult
{
    public object error { get; }

    public ErrorResult(string message, object? details = null)
    {
        error = new
        {
            message,
            details = details ?? new {}
        };
    }
}