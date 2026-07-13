using System.Diagnostics;
using Asp.Versioning;
using Auth.Application.Auth.DTOs.Requests;
using Auth.Application.Auth.DTOs.Responses;
using Auth.Application.Auth.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Shared.Kernel.Results;

namespace Auth.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("/api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;

    public AuthController(
        IAuthService authService, 
        IValidator<LoginRequest> loginRequestValidator,
        IValidator<RegisterRequest> registerRequestValidator
    )
    {
        _authService = authService;
        _loginRequestValidator = loginRequestValidator;
        _registerRequestValidator = registerRequestValidator;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ResultObject<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultObject<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultObject<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var validationResult = await _loginRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(e => e.ErrorMessage)
                );
            
            return Result<object>
                .BadRequest("1 ou mais campos inválidos", errors, Activity.Current?.Id)
                .ToActionResult();
        }
        
        var response = await _authService.LoginAsync(request);
        return response.ToActionResult();
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ResultObject<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultObject<AuthenticationResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var validationResult = await _registerRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(e => e.ErrorMessage)
                );

            return Result<object>
                .BadRequest("1 ou mais campos inválidos", errors, Activity.Current?.Id)
                .ToActionResult();
        }
        
        var response = await _authService.RegisterAsync(request);
        return response.ToActionResult();
    }
    
}