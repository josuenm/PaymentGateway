using System.Text.Json;
using Asp.Versioning;
using Catalog.Application.Products.Interfaces;
using Catalog.Application.Products.Messaging.Commands;
using Catalog.Application.Products.Services;
using Catalog.Application.Products.Validators;
using Catalog.Domain.Prices.Repositories;
using Catalog.Domain.Products.Repositories;
using Catalog.Infrastructure.Repositories;
using FluentValidation;
using MassTransit;
using Shared.Infrastructure.Configurations;
using Shared.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        
        cfg.UseRawJsonSerializer(RawSerializerOptions.AnyMessageType);
        
        cfg.Message<ProductCreatedEvent>(m =>
        {
            m.SetEntityName("catalog.product-created");
        });
    });
});

builder.Configuration.RunMigrations(typeof(ProductRepository).Assembly);
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
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductRequestValidator).Assembly);
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

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