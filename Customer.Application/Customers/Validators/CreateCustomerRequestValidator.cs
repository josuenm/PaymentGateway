using Customer.Application.Customers.DTOs.Requests;
using FluentValidation;

namespace Customer.Application.Customers.Validators;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Name);
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .EmailAddress().WithMessage("O {PropertyName} precisa ser válido");

        RuleFor(x => x.TaxId);
    }
}