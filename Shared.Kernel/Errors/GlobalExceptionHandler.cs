using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shared.Kernel.Errors;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Uma exceção não tratada ocorreu: {Message}", exception.Message);

        var statusCode = exception switch
        {
            InvalidOperationException => StatusCodes.Status500InternalServerError,
            ArgumentNullException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails()
        {
            Status = statusCode,
            Title = "Erro Interno no Servidor",
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Desculpe-nos, ocorreu um erro interno de configuração. Nossos engenheiros já foram notificados."
                : exception.Message
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}