using FluentValidation;
using PaymentLink.Application.PaymentLinks.DTOs.Requests;

namespace PaymentLink.Application.PaymentLinks.Validators;

public class CreatePaymentLinkValidator : AbstractValidator<CreatePaymentLink>
{
    public CreatePaymentLinkValidator()
    {
        RuleForEach(x => x.Methods)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .OverridePropertyName("methods");
        
        RuleForEach(x => x.Methods)
            .IsInEnum().WithMessage("O campo {PropertyName} é inválido")
            .OverridePropertyName("methods");
        
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("O cmapo {PropertyName} precisa ser fornecido")
            .OverridePropertyName("items");
        
        RuleForEach(x => x.Items)
            .SetValidator(new CreatePaymentLinkItemValidator());
    }
    
}