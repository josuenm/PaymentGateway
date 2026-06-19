using Microsoft.AspNetCore.Mvc;
using Shared.Kernel.Errors;
using Shared.Kernel.Results;

namespace Shared.Extensions;

public static class ResultExtensions
{

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Value);
        }

        var statusCode = (int)result.Error!.Type;

        return new ObjectResult(new ErrorResult(result.Error.Message, result.Error.Details))
        {
            StatusCode = statusCode
        };
    }
    
}