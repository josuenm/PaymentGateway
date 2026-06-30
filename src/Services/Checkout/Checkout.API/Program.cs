using Asp.Versioning;
using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.Interfaces;
using Checkout.Application.Checkouts.Services;
using Checkout.Infrastructure.Http.Clients;
using Checkout.Infrastructure.Http.Interfaces;

var builder = WebApplication.CreateBuilder(args);

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
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICustomerApiClient, CustomerApiClient>();
builder.Services.AddScoped<IPaymentLinkApiClient, PaymentLinkApiClient>();
builder.Services.AddScoped<IPriceApiClient, PriceApiClient>();
builder.Services.AddScoped<IPaymentApiClient, PaymentApiClient>();

builder.Services.AddHttpClient("PaymentLinkClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/api/v1/paymentlinks/internal");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});
builder.Services.AddHttpClient("PriceClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/api/v1/prices/internal");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});
builder.Services.AddHttpClient("CustomerClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/api/v1/customers/internal");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});
builder.Services.AddHttpClient("PaymentClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/api/v1/payments/internal");
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