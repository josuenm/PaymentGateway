using Auth.Application.Auth.DTOs.Responses;
using Auth.Application.Auth.Services;
using Auth.Domain.RefreshTokens.Entities;
using Auth.Domain.RefreshTokens.Repositories;
using Auth.Domain.Users.Entities;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Auth.Test.Services;

public class TokenServiceTest
{
    private IConfiguration CreateConfig(string expiration, string secret)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:AccessTokenExpirationInMinutes"] = expiration,
                ["JwtSettings:SecretKey"] = secret
            })
            .Build();
    }

    [Fact]
    public void GetAccessExpirationInMinutes_Returns20()
    {
        var config = CreateConfig("20", "SECRET_KEY_32_CHARS_MINIMUM_HERE_OK");
        var tokenService = new TokenService(config, Mock.Of<IRefreshTokenRepository>());
        
        var result = tokenService.GetAccessExpirationInMinutes();
        
        Assert.Equal(20, result);
    }

    [Fact]
    public void GetAccessExpirationInMinutes_Returns15()
    {
        var config = CreateConfig("invalido", "SECRET_KEY_32_CHARS_MINIMUM_HERE_OK");
        var tokenService = new TokenService(config, Mock.Of<IRefreshTokenRepository>());
        
        var result = tokenService.GetAccessExpirationInMinutes();
        
        Assert.Equal(15, result);
    }
    
    [Fact]
    public async Task CreateAuthenticationToken_CreatesToken()
    {
        var config = CreateConfig(
            "60", 
            "SECRET_KEY_EXAMPLE_MINIMUM_32_CHARACTERS_FOR_HMAC256"
        );
        
        var user = new User(
            "usr_123", 
            "John Doe", 
            "example@example.com", 
            "123321", 
            DateTime.Now, 
            null
        );
        user.SetRoles(new List<string> { "merchant" });

        var refreshRepo = new Mock<IRefreshTokenRepository>();
        refreshRepo
            .Setup(r => r.CreateAsync(It.IsAny<RefreshToken>()))
            .ReturnsAsync(It.IsAny<RefreshToken>());

        var tokenService = new TokenService(config, refreshRepo.Object);
        var result = await tokenService.CreateAuthenticationAsync(user);
        var objectResult = Assert.IsType<AuthenticationResponse>(result);
        
        Assert.NotNull(objectResult);
        Assert.IsType<string>(objectResult.AccessToken);
        Assert.IsType<string>(objectResult.RefreshToken);
        Assert.Equal(3600, result.ExpiresIn);
        
        Assert.Equal(user.Id, objectResult.User.Id);
        Assert.Equal(user.Name, objectResult.User.Name);
        Assert.Equal(user.Email, objectResult.User.Email);
    }
}