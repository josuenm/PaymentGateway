using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Shared.Kernel.Results;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        
        var secretKey = builder.Configuration.GetSection("JwtSettings:SecretKey").Value;
        var key = Encoding.UTF8.GetBytes(secretKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),

            ValidateIssuer = true,
            ValidIssuer = "Auth.Application",

            ValidateAudience = true,
            ValidAudience = "Auth.Application",

            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        { 
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = Result<object>.Unauthorized("As credenciais de autenticação são inválidas ou não foram fornecidas.");
                
                var json = System.Text.Json.JsonSerializer.Serialize(
                    response.ToObject(),
                    new System.Text.Json.JsonSerializerOptions 
                    { 
                        PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase 
                    }
                );

                await context.Response.WriteAsync(json);
            }
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ValidateToken", policy => policy.RequireAuthenticatedUser());
});
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(async requestContext =>
        {
            var httpContext = requestContext.HttpContext;

            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                var userId = httpContext
                    .User
                    .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                    ?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    requestContext.ProxyRequest.Headers.Remove("X-User-Id");
                    requestContext.ProxyRequest.Headers.Add("X-User-Id", userId);
                }
            }
        });
    });

var app = builder.Build();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/auth/swagger/v1/swagger.json", "Auth API");
        options.SwaggerEndpoint("/swagger/customer/swagger/v1/swagger.json", "Customer API");
        options.SwaggerEndpoint("/swagger/catalog/swagger/v1/swagger.json", "Catalog API");
        options.SwaggerEndpoint("/swagger/paymentlink/swagger/v1/swagger.json", "PaymentLink API");
        options.SwaggerEndpoint("/swagger/checkout/swagger/v1/swagger.json", "Checkout API");
        options.RoutePrefix = "swagger-ui";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();