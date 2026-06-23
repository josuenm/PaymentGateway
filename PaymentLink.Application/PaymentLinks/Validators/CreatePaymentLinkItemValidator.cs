using FluentValidation;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;

namespace PaymentLink.Application.PaymentLinks.Validators;

public class CreatePaymentLinkItemValidator : AbstractValidator<CreatePaymentLinkItem>
{
    public CreatePaymentLinkItemValidator()
    {
        RuleFor(x => x.PriceId)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .OverridePropertyName("priceId");

        RuleFor(x => x.Quantity)
            .Cascade(CascadeMode.Stop)
            .GreaterThan(0).WithMessage("O campo {PropertyName} não pode ter a quantidade zero")
            .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .OverridePropertyName("quantity");
    }
}