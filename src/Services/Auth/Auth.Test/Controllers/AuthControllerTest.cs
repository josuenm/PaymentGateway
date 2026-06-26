using Auth.API.Controllers;
using Auth.Application.Auth.DTOs.Requests;
using Auth.Application.Auth.DTOs.Responses;
using Auth.Application.Auth.Interfaces;
using Auth.Application.Auth.Validators;
using Auth.Application.Users.DTOs.Responses;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Kernel.Results;

namespace Auth.Test.Controllers;

public class AuthControllerTest
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTest()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(
            _authServiceMock.Object, 
            new LoginRequestValidator(), 
            new RegisterRequestValidator()
        );
    }

    [Fact]
    public async Task Login_WithInvalidData_ReturnsBadRequest()
    {
        var request = new LoginRequest("", "123321");
        
        var result = await _authController.LoginAsync(request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);

        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);
        
        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);
        
        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("email", resultValue.Error.Details.Keys);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        var request = new LoginRequest("example@example.com", "123321");

        _authServiceMock
            .Setup(service => service.LoginAsync(request))
            .ReturnsAsync(Result<AuthenticationResponse>.Ok(
                new AuthenticationResponse(
                    "ACCESS_TOKEN", 
                    "REFRESH_TOKEN", 
                    900, 
                    new UserResponse("usr_123", "John Doe", request.Email)
                )
            ));
        
        var result = await _authController.LoginAsync(request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<AuthenticationResponse>>(objectResult.Value);
        
        Assert.Equal(200, objectResult.StatusCode);
        Assert.True(resultValue.Success);
        
        Assert.NotNull(resultValue.Data);
        Assert.NotNull(resultValue.Data.User);
        
        Assert.Equal("ACCESS_TOKEN", resultValue.Data.AccessToken);
        Assert.Equal("REFRESH_TOKEN", resultValue.Data.RefreshToken);
        Assert.Equal(900, resultValue.Data.ExpiresIn);
        
        Assert.Equal("usr_123", resultValue.Data.User.Id);
        Assert.Equal("John Doe", resultValue.Data.User.Name);
        Assert.Equal(request.Email, resultValue.Data.User.Email);
    }

    [Fact]
    public async Task Register_WithInvalidData_ReturnsBadRequest()
    {
        var request = new RegisterRequest("John Doe", "", "123321");
        
        var result = await _authController.RegisterAsync(request);
        
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<object>>(objectResult.Value);
        
        Assert.Equal(400, objectResult.StatusCode);
        Assert.False(resultValue.Success);
        
        Assert.NotNull(resultValue.Error);
        Assert.NotNull(resultValue.Error.Details);
        
        Assert.Equal("1 ou mais campos inválidos", resultValue.Error.Message);
        Assert.Contains("email", resultValue.Error.Details.Keys);
    }

    [Fact]
    public async Task Register_WIthValidData_ReturnsSuccess()
    {
        var request = new RegisterRequest("Joe Doe", "example@example.com", "123321");

        _authServiceMock
            .Setup(service => service.RegisterAsync(request))
            .ReturnsAsync(Result<AuthenticationResponse>.Created(
                new AuthenticationResponse(
                    "ACCESS_TOKEN",
                    "REFRESH_TOKEN",
                    900, 
                    new UserResponse("usr_123", "John Doe", request.Email)
                )
            ));
        
        var result = await _authController.RegisterAsync(request);
        var objectResult = Assert.IsType<ObjectResult>(result);
        var resultValue = Assert.IsType<ResultObject<AuthenticationResponse>>(objectResult.Value);
        
        Assert.Equal(201, objectResult.StatusCode);
        Assert.True(resultValue.Success);
        
        Assert.NotNull(resultValue.Data);
        Assert.NotNull(resultValue.Data.User);
        
        Assert.Equal("ACCESS_TOKEN", resultValue.Data.AccessToken);
        Assert.Equal("REFRESH_TOKEN", resultValue.Data.RefreshToken);
        Assert.Equal(900, resultValue.Data.ExpiresIn);
        
        Assert.Equal("usr_123", resultValue.Data.User.Id);
        Assert.Equal("John Doe", resultValue.Data.User.Name);
        Assert.Equal(request.Email, resultValue.Data.User.Email);
    }
}