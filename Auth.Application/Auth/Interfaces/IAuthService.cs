using Auth.Application.Auth.DTOs.Requests;
using Auth.Application.Auth.DTOs.Responses;
using Shared.Kernel.Results;

namespace Auth.Application.Auth.Interfaces;

public interface IAuthService
{
    public Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request);
    public Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request);
}