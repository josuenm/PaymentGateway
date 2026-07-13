using Asp.Versioning;
using Auth.Application.Auth.Interfaces;
using Auth.Application.Auth.Services;
using Auth.Application.Auth.Validators;
using Auth.Domain.RefreshTokens.Repositories;
using Auth.Domain.Roles.Repositories;
using Auth.Domain.Users.Interfaces;
using Auth.Domain.Users.Repositories;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Security.Cryptography;
using FluentValidation;
using Shared.Infrastructure.Configurations;
using Shared.Infrastructure.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.RunMigrations(typeof(UserRepository).Assembly);
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
builder.Services.AddValidatorsFromAssembly(typeof(LoginRequestValidator).Assembly);

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseDeveloperExceptionPage();

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