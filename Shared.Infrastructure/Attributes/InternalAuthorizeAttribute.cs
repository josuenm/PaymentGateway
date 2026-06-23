using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Infrastructure.Attributes;


[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class InternalAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Internal-Key", out var extractedKey))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
            return;
        }

        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var actualKey = config["InternalSettings:ApiKey"];

        if (actualKey != extractedKey)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        await next();
    }
}