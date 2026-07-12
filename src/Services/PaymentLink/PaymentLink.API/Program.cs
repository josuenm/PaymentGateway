using System.Text.Json;
using Asp.Versioning;
using FluentValidation;
using MassTransit;
using PaymentLink.Application.PaymentLinks.Interfaces;
using PaymentLink.Application.PaymentLinks.Messaging.Commands;
using PaymentLink.Application.PaymentLinks.Services;
using PaymentLink.Application.PaymentLinks.Validators;
using PaymentLink.Domain.PaymentLinks.Repositories;
using PaymentLink.Infrastructure.Data.TypeHandlers;
using PaymentLink.Infrastructure.Repositories;
using Shared.Infrastructure.Configurations;
using Shared.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
        
        cfg.Message<PaymentLinkCreatedCommand>(m =>
        {
            m.SetEntityName("paymentlink.paymentlink-created");
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
Dapper.SqlMapper.AddTypeHandler(new PaymentLinkMethodsHandler());
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddValidatorsFromAssembly(typeof(CreatePaymentLinkValidator).Assembly);
builder.Services.AddScoped<IPaymentLinkService, PaymentLinkService>();
builder.Services.AddScoped<IPaymentLinkRepository, PaymentLinkRepository>();
builder.Services.AddScoped<IPaymentLinkItemRepository, PaymentLinkItemRepository>();
builder.Services.AddHttpClient("CatalogClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001");
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