using Asp.Versioning;
using Customer.Application.Customers.Messaging.Events;
using Customer.Application.Customers.Messaging.Commands;
using Customer.Application.Customers.Interfaces;
using Customer.Application.Customers.Services;
using Customer.Application.Customers.Validators;
using Customer.Domain.Customers.Repositories;
using Customer.Infrastructure.Messaging.Consumers;
using Shared.Infrastructure.Contexts;
using Shared.Infrastructure.Configurations;
using Customer.Infrastructure.Repositories;
using FluentValidation;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CustomerCreationConsumer>();
    x.AddConsumer<CustomerUpdateConsumer>();
    
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", host =>
        {
            host.Username("guest");
            host.Password("guest");
        });
        
        cfg.Message<CustomerCreatedCommand>(m =>
        {
            m.SetEntityName("customer.customer-created");
        });


        cfg.Message<CustomerCreatedEvent>(m =>
        {
            m.SetEntityName("customer.customer-creation-requested");
        });
        cfg.Message<CustomerUpdatedEvent>(m =>
        {
            m.SetEntityName("customer.customer-update-requested");
        });

        
        cfg.ReceiveEndpoint("customer-creation", e =>
        {
            e.ConfigureConsumer<CustomerCreationConsumer>(ctx);
        });
        cfg.ReceiveEndpoint("customer-update", e =>
        {
            e.ConfigureConsumer<CustomerUpdateConsumer>(ctx);
        });
    });
});

builder.Configuration.RunMigrations(typeof(CustomerRepository).Assembly);
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

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddValidatorsFromAssembly(typeof(CreateCustomerRequestValidator).Assembly);
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

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