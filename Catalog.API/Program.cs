using System.Text.Json;
using Asp.Versioning;
using Catalog.Application.Products.Interfaces;
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
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseUpper));
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductRequestValidator).Assembly);
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();