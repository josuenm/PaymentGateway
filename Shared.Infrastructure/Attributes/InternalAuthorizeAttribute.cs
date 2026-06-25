using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.Results;

namespace Shared.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class InternalAuthorizeAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("X-Internal-Key", out var extractedKey))
        {
            context.Result = Result<object>.Unauthorized("Chave interna não encontrada").ToActionResult();
            return;
        }

        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var actualKey = config["InternalSettings:ApiKey"];

        if (actualKey != extractedKey)
        {
            context.Result = Result<object>.Forbidden("Você não tem permissão para a rota").ToActionResult();
            return;
        }

        await next();
    }
}