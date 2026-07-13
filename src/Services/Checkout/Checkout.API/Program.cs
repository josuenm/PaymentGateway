using System.Text.Json;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Checkout.Application.Checkouts.Abstractions;
using Checkout.Application.Checkouts.Interfaces;
using Checkout.Application.Checkouts.Services;
using Checkout.Application.Checkouts.Validators;
using Checkout.Application.CustomerReadModels.Interfaces;
using Checkout.Application.CustomerReadModels.Messaging.Events;
using Checkout.Application.CustomerReadModels.Messaging.Commands;
using Checkout.Application.CustomerReadModels.Services;
using Checkout.Application.PaymentLinkReadModels.Messaging.Events;
using Checkout.Application.PaymentLinkReadModels.Interfaces;
using Checkout.Application.PaymentLinkReadModels.Services;
using Checkout.Application.ProductReadModels.Interfaces;
using Checkout.Application.ProductReadModels.Messaging.Events;
using Checkout.Application.ProductReadModels.Services;
using Checkout.Domain.Checkouts.Repositories;
using Checkout.Domain.CustomerReadModels.Repositories;
using Checkout.Domain.PaymentLinkReadModels.Repositories;
using Checkout.Domain.ProductReadModels.Repositories;
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
    x.AddConsumer<ProductCreatedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
        
        cfg.ConfigureJsonSerializerOptions(options =>
        {
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        });
        cfg.UseRawJsonSerializer(RawSerializerOptions.AnyMessageType);
        
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
        cfg.Message<ProductCreatedEvent>(m =>
        {
            m.SetEntityName("catalog.product-created");
        });


        cfg.ReceiveEndpoint("checkout-customer-created", e =>
        {
            e.ConfigureConsumer<CustomerCreatedConsumer>(ctx);
        });
        cfg.ReceiveEndpoint("checkout-paymentlink-created", e =>
        {
            e.ConfigureConsumer<PaymentLinkCreatedConsumer>(ctx);
        });
        cfg.ReceiveEndpoint("checkout-product-created", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(ctx);
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
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
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
builder.Services.AddScoped<IPaymentLinkItemReadModelRepository, PaymentLinkItemReadModelRepository>();
builder.Services.AddScoped<IProductReadModelService, ProductReadModelService>();
builder.Services.AddScoped<IProductReadModelRepository, ProductReadModelRepository>();
builder.Services.AddScoped<IPriceReadModelService, PriceReadModelService>();
builder.Services.AddScoped<IPriceReadModelRepository, PriceReadModelRepository>();

builder.Services.AddHttpClient<IPaymentApiClient, PaymentApiClient>("PaymentClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
    client.DefaultRequestHeaders.Add("X-Internal-Key", builder.Configuration["InternalSettings:ApiKey"]);
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();