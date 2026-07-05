using Checkout.Application.Checkouts.DTOs.Requests;
using FluentValidation;

namespace Checkout.Application.Checkouts.Validators;

public class CreatePaymentRequestValidator : AbstractValidator<CreatePaymentRequest>
{
    public CreatePaymentRequestValidator()
    {
        RuleFor(x => x.SourceId)
            .NotNull().WithMessage("O sourceId precisa ser fornecido")
            .OverridePropertyName("sourceId");

        RuleFor(x => x.Method)
            .NotNull().WithMessage("O método de pagamento precisa ser fornecido")
            .IsInEnum().WithMessage("O método de pagamento precisa ser válido")
            .OverridePropertyName("method");

        RuleFor(x => x.Customer)
            .NotNull().WithMessage("Os dados do cliente precisa ser fornecido");
        
        RuleFor(x => x.Customer.Email)
            .NotNull().WithMessage("O email precisa ser fornecido")
            .EmailAddress().WithMessage("O email precisa ser válido")
            .OverridePropertyName("email");

        RuleFor(x => x.Customer.Name)
            .NotEmpty().WithMessage("O nome precisa ser fornecido")
            .OverridePropertyName("name");

        RuleFor(x => x.Customer.TaxId)
            .NotNull().WithMessage("O documento precisa ser fornecido")
            .Length(11, 14).WithMessage("O documento precisa ser válido")
            .OverridePropertyName("taxId");
    }
}