using Auth.Application.Auth.DTOs.Responses;
using Auth.Domain.Users.Entities;

namespace Auth.Application.Auth.Interfaces;

public interface ITokenService
{
    public int GetAccessExpirationInMinutes();
    public Task<AuthenticationResponse> CreateAuthenticationAsync(User user);
}