using Auth.Application.Auth.DTOs.Requests;
using Auth.Application.Auth.DTOs.Responses;
using Auth.Application.Auth.Interfaces;
using Auth.Domain.Roles.Repositories;
using Auth.Domain.Users.Entities;
using Auth.Domain.Users.Interfaces;
using Auth.Domain.Users.Repositories;
using Shared.Kernel.Results;

namespace Auth.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(
        ITokenService tokenService, 
        IUserRepository userRepository, 
        IRoleRepository roleRepository, 
        IPasswordHasher passwordHasher)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);

        if (user == null)
            return Result<AuthenticationResponse>.Failure("Usuário não existe", ErrorType.NotFound);

        if (!_passwordHasher.VerifyPassword(request.Password, user.Password))
            return Result<AuthenticationResponse>.Failure("Email ou senha incorretos", ErrorType.Unauthorized);

        var authResponse = await _tokenService.CreateAuthenticationAsync(user); 
        
        return Result<AuthenticationResponse>.Ok(authResponse);
    }

    public async Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request)
    {
        var userFound = await _userRepository.GetUserByEmailAsync(request.Email);

        if (userFound != null)
            return Result<AuthenticationResponse>.Failure("Usuário ja existe", ErrorType.Conflict);

        var user = new User(
            request.Name,
            request.Email,
            _passwordHasher.HashPassword(request.Password)
        );
        
        var role = await _roleRepository.GetRoleByName("merchant");

        if (role == null)
            throw new InvalidOperationException("Erro interno, a role não foi encontrada");
        
        user.SetRoles(new List<string> { role.Name });

        await _userRepository.CreateAsync(user);
        
        var authResponse = await _tokenService.CreateAuthenticationAsync(user);

        return Result<AuthenticationResponse>.Ok(authResponse);
    }
}