using Auth.Application.Auth.DTOs.Requests;
using Auth.Application.Auth.DTOs.Responses;
using Auth.Application.Auth.Interfaces;
using Auth.Application.Auth.Services;
using Auth.Application.Users.DTOs.Responses;
using Auth.Domain.Roles.Entities;
using Auth.Domain.Roles.Repositories;
using Auth.Domain.Users.Entities;
using Auth.Domain.Users.Interfaces;
using Auth.Domain.Users.Repositories;
using Moq;
using Shared.Kernel.Results;

namespace Auth.Test.Services;

public class AuthServiceTest
{
    private readonly AuthService _authService;
    private readonly Mock<ITokenService> _tokenService;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IRoleRepository> _roleRepository;
    private readonly Mock<IPasswordHasher> _passwordHasher;

    public AuthServiceTest()
    {
        _tokenService = new Mock<ITokenService>();
        _userRepository = new Mock<IUserRepository>();
        _roleRepository = new Mock<IRoleRepository>();
        _passwordHasher = new Mock<IPasswordHasher>();

        _authService = new AuthService(
            _tokenService.Object,
            _userRepository.Object,
            _roleRepository.Object, 
            _passwordHasher.Object
        );
    }

    [Theory]
    [InlineData("example2@example.com", "111111", "Usuário não existe")]
    [InlineData("example@example.com", "111111", "Email ou senha incorretos")]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized(
        string email, 
        string password, 
        string expectedErrorMessage
    )
    {
        var request = new LoginRequest(email, password);
        
        var correctUser = new User(
            "usr_123", 
            "John Doe", 
            "example@example.com", 
            "123321", 
            DateTime.Now, 
            null
        );

        _userRepository
            .Setup(method => method.GetUserByEmailAsync(request.Email))
            .ReturnsAsync(request.Email == correctUser.Email ? correctUser : null);

        _passwordHasher
            .Setup(method => method.VerifyPassword(request.Password, correctUser.Password))
            .Returns(false);

        var result = await _authService.LoginAsync(request);

        var resultObject = Assert.IsType<Result<AuthenticationResponse>>(result);

        Assert.Equal(401, resultObject.StatusCode);
        Assert.Null(resultObject.Data);
        Assert.NotNull(resultObject.Error);
        Assert.False(resultObject.Success);
        Assert.Equal(expectedErrorMessage, resultObject.Error.Message);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        var request = new LoginRequest("example@example.com", "123321");
        
        var correctUser = new User(
            "usr_123", 
            "John Doe", 
            "example@example.com", 
            "123321", 
            DateTime.Now, 
            null
        );
        
        _userRepository
            .Setup(method => method.GetUserByEmailAsync(request.Email))
            .ReturnsAsync(request.Email == correctUser.Email ? correctUser : null);

        _passwordHasher
            .Setup(method => method.VerifyPassword(request.Password, correctUser.Password))
            .Returns(request.Password == correctUser.Password);

        _tokenService
            .Setup(method => method.CreateAuthenticationAsync(correctUser))
            .ReturnsAsync(new AuthenticationResponse(
                "ACCESS_TOKEN", 
                "REFRSH_TOKEN",
                900, 
                new UserResponse(correctUser.Id, correctUser.Name, correctUser.Email)
            ));
        
        var result = await _authService.LoginAsync(request);
        var resultObject = Assert.IsType<Result<AuthenticationResponse>>(result);
        
        Assert.Equal(200, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        Assert.Equal("ACCESS_TOKEN", resultObject.Data.AccessToken);
        Assert.Equal("REFRSH_TOKEN", resultObject.Data.RefreshToken);
        Assert.Equal(900, resultObject.Data.ExpiresIn);
        
        Assert.Equal(correctUser.Id, resultObject.Data.User.Id);
        Assert.Equal(correctUser.Name, resultObject.Data.User.Name);
        Assert.Equal(correctUser.Email, resultObject.Data.User.Email);
    }

    [Fact]
    public async Task Register_WithExistingUser_ReturnsConflict()
    {
        var user = new User(
            "usr_123", 
            "John Doe", 
            "example@example.com", 
            "123321", 
            DateTime.Now, 
            null
        );
        
        var request = new RegisterRequest(user.Name, user.Email, user.Password);
        
        _userRepository
            .Setup(method => method.GetUserByEmailAsync(request.Email))
            .ReturnsAsync(user);
        
        var result = await _authService.RegisterAsync(request);
        var resultObject = Assert.IsType<Result<AuthenticationResponse>>(result);
        
        Assert.Equal(409, resultObject.StatusCode);
        Assert.Null(resultObject.Data);
        Assert.NotNull(resultObject.Error);
        Assert.False(resultObject.Success);
        Assert.Equal("Usuário ja existe", resultObject.Error.Message);
    }

    [Fact]
    public async Task Register_WithNonRole_ReturnsInternalServerError()
    {
        var user = new UserResponse(
            "usr_123", 
            "John Doe", 
            "example@example.com"
        );
        
        var request = new RegisterRequest(user.Name, user.Email, "123321");
        
        _userRepository
            .Setup(method => method.GetUserByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);

        _roleRepository
            .Setup(method => method.GetRoleByName("merchant"))
            .ReturnsAsync((Role?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(request)
        );
        
        Assert.Equal("Erro interno, a role não foi encontrada", exception.Message);
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreated()
    {
        var user = new UserResponse(
            "usr_123", 
            "John Doe", 
            "example@example.com"
        );
        
        var request = new RegisterRequest(user.Name, user.Email, "123321");
        
        _userRepository
            .Setup(method => method.GetUserByEmailAsync(request.Email))
            .ReturnsAsync((User?)null);
        
        _userRepository
            .Setup(method => method.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(It.IsAny<User>());

        _roleRepository
            .Setup(method => method.GetRoleByName("merchant"))
            .ReturnsAsync(new Role("role_123", "merchant"));
        
        _tokenService
            .Setup(method => method.CreateAuthenticationAsync(It.IsAny<User>()))
            .ReturnsAsync(new AuthenticationResponse(
                "ACCESS_TOKEN", 
                "REFRSH_TOKEN",
                900, 
                new UserResponse(user.Id, user.Name, user.Email)
            ));
        
        _passwordHasher
            .Setup(method => method.HashPassword(request.Password))
            .Returns("HASHED_PASSWORD");
        
        var result = await _authService.RegisterAsync(request);
        var resultObject = Assert.IsType<Result<AuthenticationResponse>>(result);
        
        Assert.Equal(201, resultObject.StatusCode);
        Assert.NotNull(resultObject.Data);
        Assert.Null(resultObject.Error);
        Assert.True(resultObject.Success);
        
        Assert.Equal("ACCESS_TOKEN", resultObject.Data.AccessToken);
        Assert.Equal("REFRSH_TOKEN", resultObject.Data.RefreshToken);
        Assert.Equal(900, resultObject.Data.ExpiresIn);
        Assert.Equal(user.Id, resultObject.Data.User.Id);
        Assert.Equal(user.Name, resultObject.Data.User.Name);
        Assert.Equal(user.Email, resultObject.Data.User.Email);
    }
}