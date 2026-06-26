using Catalog.Application.Prices.Validators;
using Catalog.Application.Products.DTOs.Requests;
using FluentValidation;

namespace Catalog.Application.Products.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .OverridePropertyName("name");

        RuleFor(x => x.Description);

        RuleFor(x => x.Metadata);

        When(x => x.Prices != null, () =>
        {
            RuleForEach(x => x.Prices)
                .SetValidator(new CreatePriceRequestValidator());
        });
    }
}