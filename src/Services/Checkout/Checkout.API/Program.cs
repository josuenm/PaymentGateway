using System.Text.Json;
using Asp.Versioning;
using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.Interfaces;
using Checkout.Application.Checkouts.Services;
using Checkout.Application.Checkouts.Validators;
using Checkout.Application.CustomerReadModels.Interfaces;
using Checkout.Application.CustomerReadModels.Messaging.Events;
using Checkout.Application.CustomerReadModels.Messaging.Commands;
using Checkout.Application.CustomerReadModels.Services;
using Checkout.Application.PaymentLinkReadModels.Events.Events;
using Checkout.Application.PaymentLinkReadModels.Interfaces;
using Checkout.Application.PaymentLinkReadModels.Services;
using Checkout.Domain.Checkouts.Repositories;
using Checkout.Domain.CustomerReadModels.Repositories;
using Checkout.Domain.PaymentLinkReadModels.Repositories;
using Checkout.Infrastructure.Http.Clients;
using Checkout.Infrastructure.Http.Interfaces;
using Checkout.Infrastructure.Messaging.Consumers;
using Checkout.Infrastructure.Repositories;
using FluentValidation;
using MassTransit;
using Shared.Infrastructure.Configurations;
using Shared.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CustomerCreatedConsumer>();
    x.AddConsumer<PaymentLinkCreatedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });

        
        cfg.Message<CreateCustomerCommand>(m =>
        {
            m.SetEntityName("customer.customer-creation-requested");
        });
        cfg.Message<UpdateCustomerCommand>(m =>
        {
            m.SetEntityName("customer.customer-update-requested");
        });
        

        cfg.Message<CustomerCreatedEvent>(m =>
        {
            m.SetEntityName("customer.customer-created");
        });
        cfg.Message<PaymentLinkCreatedEvent>(m =>
        {
            m.SetEntityName("paymentlink.paymentlink-created");
        });


        cfg.ReceiveEndpoint("checkout-customer-created", e =>
        {
            e.ConfigureConsumer<CustomerCreatedConsumer>(ctx);
        });
        cfg.ReceiveEndpoint("checkout-paymentlink-created", e =>
        {
            e.ConfigureConsumer<PaymentLinkCreatedConsumer>(ctx);
        });
    });
});
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
builder.Services.AddScoped<ICustomerReadModelService, CustomerReadModelService>();
builder.Services.AddScoped<ICustomerReadModelRepository, CustomerReadModelRepository>();
builder.Services.AddScoped<IPaymentLinkReadModelService, PaymentLinkReadModelService>();
builder.Services.AddScoped<IPaymentLinkReadModelRepository, PaymentLinkReadModelRepository>();

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