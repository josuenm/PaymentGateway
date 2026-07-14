using System.Text.Json;
using Asp.Versioning;
using Dapper;
using FluentValidation;
using MassTransit;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Application.PaymentLinks.Messaging.Commands;
using PaymentLink.Application.PaymentLinks.Services;
using PaymentLink.Application.PaymentLinks.Validators;
using PaymentLink.Application.ProductReadModels.Interfaces;
using PaymentLink.Application.ProductReadModels.Messaging.Events;
using PaymentLink.Application.ProductReadModels.Services;
using PaymentLink.Domain.PaymentLinks.Enums;
using PaymentLink.Domain.PaymentLinks.Repositories;
using PaymentLink.Domain.ProductReadModels.Repositories;
using PaymentLink.Infrastructure.Messaging.Consumers;
using PaymentLink.Infrastructure.Repositories;
using PaymentLink.Infrastructure.Repositories.TypeHandlers;
using Shared.Infrastructure.Configurations;
using Shared.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>();
    
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
        
        cfg.UseRawJsonSerializer(RawSerializerOptions.AnyMessageType);
        
        cfg.Message<PaymentLinkCreatedCommand>(m =>
        {
            m.SetEntityName("paymentlink.paymentlink-created");
        });
        
        
        cfg.Message<ProductCreatedEvent>(m =>
        {
            m.SetEntityName("catalog.product-created");
        });
        
        
        cfg.ReceiveEndpoint("paymentlink-product-created", e =>
        {
            e.ConfigureConsumer<ProductCreatedConsumer>(ctx);
        });
    });
});
builder.Configuration.RunMigrations(typeof(PaymentLinkRepository).Assembly);
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddValidatorsFromAssembly(typeof(CreatePaymentLinkValidator).Assembly);
SqlMapper.AddTypeHandler(new JsonTypeHandler<IReadOnlyCollection<PaymentLinkMethods>>());

builder.Services.AddScoped<IPaymentLinkService, PaymentLinkService>();
builder.Services.AddScoped<IPaymentLinkRepository, PaymentLinkRepository>();
builder.Services.AddScoped<IPaymentLinkItemRepository, PaymentLinkItemRepository>();
builder.Services.AddScoped<IProductReadModelService, ProductReadModelService>();
builder.Services.AddScoped<IPriceReadModelService, PriceReadModelService>();
builder.Services.AddScoped<IProductReadModelRepository, ProductReadModelRepository>();
builder.Services.AddScoped<IPriceReadModelRepository, PriceReadModelRepository>();

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