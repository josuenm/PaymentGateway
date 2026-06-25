using System.Text.Json;
using Asp.Versioning;
using FluentValidation;
using MassTransit;
using PaymentLink.API.Messaging.Consumers;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Application.PaymentLinks.Services;
using PaymentLink.Application.PaymentLinks.Validators;
using PaymentLink.Application.PriceReplicas.Interfaces;
using PaymentLink.Application.PriceReplicas.Services;
using PaymentLink.Domain.PaymentLinks.Repositories;
using PaymentLink.Domain.PriceReplicas.Repositories;
using PaymentLink.Infrastructure.Data.TypeHandlers;
using PaymentLink.Infrastructure.Repositories;
using Shared.Infrastructure.Configurations;
using Shared.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>();
    
    if (builder.Environment.IsDevelopment())
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });
            cfg.ConfigureEndpoints(context); 
        });
    }
    else
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetConnectionString("AzureServiceBus"));
            cfg.ConfigureEndpoints(context);
        });
    }
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
Dapper.SqlMapper.AddTypeHandler(new PaymentLinkMethodsHandler());
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddValidatorsFromAssembly(typeof(CreatePaymentLinkValidator).Assembly);
builder.Services.AddScoped<IPaymentLinkService, PaymentLinkService>();
builder.Services.AddScoped<IPriceReplicaService, PriceReplicaService>();
builder.Services.AddScoped<IPaymentLinkRepository, PaymentLinkRepository>();
builder.Services.AddScoped<IPaymentLinkItemRepository, PaymentLinkItemRepository>();
builder.Services.AddScoped<IPriceReplicaRepository, PriceReplicaRepository>();

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