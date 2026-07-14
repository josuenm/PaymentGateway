using Checkout.Application.Checkouts.DTOs.Requests;
using Checkout.Domain.Checkouts.Enums;
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

        When(x => x.Method == PaymentMethod.CreditCard, () =>
        {
            RuleFor(x => x.Card)
                .NotNull().WithMessage("O cartão precisa ser fornecido")
                .OverridePropertyName("card")
                .DependentRules(() =>
                {
                    RuleFor(x => x.Card!.HolderName)
                        .NotEmpty().WithMessage("O nome do dono do cartão precisa ser fornecido");

                    RuleFor(x => x.Card!.Number)
                        .NotEmpty().WithMessage("O número do cartão precisa ser fornecido")
                        .Length(13, 19).WithMessage("Cartão inválido");

                    RuleFor(x => x.Card!.Cvv)
                        .NotEmpty().WithMessage("O CVV precisa ser fornecido")
                        .Length(3).WithMessage("CVV inválido");

                    RuleFor(x => x.Card!.ExpiryDate)
                        .NotEmpty().WithMessage("O expiro date deve ser fornecido");
                });
        });
    }
}