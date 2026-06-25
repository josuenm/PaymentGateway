using FluentValidation;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;

namespace PaymentLink.Application.PaymentLinks.Validators;

public class CreatePaymentLinkValidator : AbstractValidator<CreatePaymentLink>
{
    public CreatePaymentLinkValidator()
    {
        RuleFor(x => x.Methods)
            .NotNull()
            .WithMessage("O campo {PropertyName} precisa ser fornecido")
            .OverridePropertyName("methods");
        
        RuleForEach(x => x.Methods)
            .IsInEnum()
            .WithMessage("O campo {PropertyName} é inválido")
            .OverridePropertyName("methods");
        
        RuleForEach(x => x.Items)
            .SetValidator(new CreatePaymentLinkItemValidator());
    }
    
}