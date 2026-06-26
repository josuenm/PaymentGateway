using Auth.Application.Auth.DTOs.Requests;
using FluentValidation;

namespace Auth.Application.Auth.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .EmailAddress().WithMessage("O {PropertyName} precisa ser válido")
            .OverridePropertyName("email");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .Length(6, 100).WithMessage("O campo {PropertyName} deve conter entre 6 e 100 caracteres")
            .OverridePropertyName("password");;
    }
}