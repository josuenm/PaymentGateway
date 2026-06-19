using Microsoft.Extensions.DependencyInjection;
using Shared.Kernel.Errors;

namespace Shared.Extensions;

public static partial class DependencyInjection
{
    public static IServiceCollection AddApiErrorHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.Remove("exception");
                context.ProblemDetails.Extensions.Remove("headers");
            };
        });
        
        return services;
    }
}