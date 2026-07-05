using System.Text.Json;
using Asp.Versioning;
using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.Interfaces;
using Checkout.Application.Checkouts.Services;
using Checkout.Application.Checkouts.Validators;
using Checkout.Domain.Checkouts.Repositories;
using Checkout.Infrastructure.Http.Clients;
using Checkout.Infrastructure.Http.Interfaces;
using Checkout.Infrastructure.Repositories;
using FluentValidation;
using Shared.Infrastructure.Configurations;
using Shared.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.RunMigrations(typeof(CheckoutSessionRepository).Assembly);
builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddValidatorsFromAssembly(typeof(CreatePaymentRequestValidator).Assembly);
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICheckoutSessionRepository, CheckoutSessionRepository>();
builder.Services.AddScoped<ICustomerApiClient, CustomerApiClient>();
builder.Services.AddScoped<IPaymentLinkApiClient, PaymentLinkApiClient>();
builder.Services.AddScoped<IPriceApiClient, PriceApiClient>();
builder.Services.AddScoped<IPaymentApiClient, PaymentApiClient>();

builder.Services.AddHttpClient<IPaymentLinkApiClient, PaymentLinkApiClient>("PaymentLinkClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});
builder.Services.AddHttpClient<IPriceApiClient, PriceApiClient>("PriceClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});
builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>("CustomerClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});
builder.Services.AddHttpClient<IPaymentApiClient, PaymentApiClient>("PaymentClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();