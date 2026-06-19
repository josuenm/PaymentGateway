using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Auth.DTOs.Responses;
using Auth.Application.Auth.Interfaces;
using Auth.Application.Users.DTOs.Responses;
using Auth.Domain.RefreshTokens.Entities;
using Auth.Domain.RefreshTokens.Repositories;
using Auth.Domain.Users.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Auth.Services;

public class TokenService : ITokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly string _jwtSecret;
    private readonly string _accessTokenExpirationInMinutes;
    
    public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository)
    {
        _accessTokenExpirationInMinutes = configuration.GetValue<string>("JwtSettings:AccessTokenExpirationInMinutes") 
                     ?? throw new ArgumentNullException(nameof(configuration), "A expiração do access é obrigatória");
        
        _jwtSecret = configuration.GetValue<string>("JwtSettings:SecretKey") 
                     ?? throw new ArgumentNullException(nameof(configuration), "O secret do JWT é obrigatório");
        
        _refreshTokenRepository = refreshTokenRepository;
    }

    public int GetAccessExpirationInMinutes()
    {
        if (int.TryParse(_accessTokenExpirationInMinutes, out int minutes)) return minutes;

        return 15;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        return Convert.ToBase64String(randomNumber)
            .Replace("/", "-")
            .Replace("+", "_")
            .TrimEnd('=');
    }
    
    public async Task<AuthenticationResponse> CreateAuthenticationAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _jwtSecret;
        var key = Encoding.UTF8.GetBytes(secretKey);

        var expirationInMinutes = GetAccessExpirationInMinutes();
        var expirationInSeconds = expirationInMinutes * 60;
        var expiresAt = DateTime.UtcNow.AddMinutes(expirationInMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt, 
            Issuer = "Auth.Application", 
            Audience = "Auth.Application", 
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        var refreshToken = new RefreshToken(
            GenerateRefreshToken(), 
            user.Id, 
            false, 
            DateTime.UtcNow.AddDays(7)
        );
        await _refreshTokenRepository.CreateAsync(refreshToken);
        
        var userResponse = new UserResponse(user.Id, user.Name, user.Email);
        
        return new AuthenticationResponse(
            accessToken, 
            refreshToken.Token, 
            expirationInSeconds, 
            userResponse
        );
    }
}