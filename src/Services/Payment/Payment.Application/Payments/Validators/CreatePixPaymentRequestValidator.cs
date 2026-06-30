using FluentValidation;
using Payment.Application.Payments.DTOs.Requests;

namespace Payment.Application.Payments.Validators;

public class CreatePixPaymentRequestValidator : AbstractValidator<CreatePixPaymentRequest>
{
    public CreatePixPaymentRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull().WithMessage("O id do usuário precisa ser fornecido");

        RuleFor(x => x.LiveMode)
            .NotNull().WithMessage("O live mode precisa ser fornecido");
        
        RuleFor(x => x.Method)
            .NotNull().WithMessage("O método precisa ser fornecido")
            .IsInEnum().WithMessage("O método precisa ser válido")
            .OverridePropertyName("method");
        
        RuleFor(x => x.Amount)
            .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser maior que zero")
            .OverridePropertyName("amount");

        RuleFor(x => x.Currency)
            .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .Must(currency => currency?.ToUpper() == "BRL")
            .WithMessage("O campo {PropertyName} precisa ser brl ou BRL")
            .OverridePropertyName("currency");


        RuleFor(x => x.Customer.Id)
            .NotNull().WithMessage("O id do cliente precisa ser fornecido");
        
        RuleFor(x => x.Customer.TaxId)
            .NotNull().WithMessage("O documento do cliente precisa ser fornecido");
        
        RuleFor(x => x.Customer.Name)
            .NotNull().WithMessage("O nome do cliente precisa ser fornecido");
        
        RuleFor(x => x.Customer.Email)
            .NotNull().WithMessage("O email do cliente precisa ser fornecido");
    }
}