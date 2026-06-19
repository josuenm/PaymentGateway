using Catalog.Application.Prices.DTOs.Requests;
using Catalog.Domain.Enums;
using FluentValidation;

namespace Catalog.Application.Prices.Validators;

public class CreatePriceRequestValidator : AbstractValidator<CreatePriceRequest>
{
    public CreatePriceRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .OverridePropertyName("name");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .GreaterThan(0).WithMessage("O campo {PropertyName} precisa ser maior que zero")
            .OverridePropertyName("amount");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .Must(currency => currency?.ToUpper() == "BRL")
            .WithMessage("O campo {PropertyName} precisa ser brl ou BRL")
            .OverridePropertyName("currency");

        RuleFor(x => x.Frequency)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
            .IsInEnum().WithMessage("O campo {PropertyName} é inválido")
            .OverridePropertyName("frequency");;

        RuleFor(x => x.Cycle)
            .IsInEnum().When(x => x.Cycle.HasValue).WithMessage("O campo {PropertyName} é inválido")
            .OverridePropertyName("cycle");

        When(x => x.Frequency == PriceFrequency.Recurring, () =>
        {
            RuleFor(x => x.Cycle)
                .NotNull().WithMessage("O campo {PropertyName} precisa ser fornecido quando a frequência for recorrente")
                .IsInEnum().WithMessage("O campo {PropertyName} é inválido")
                .OverridePropertyName("cycle");
        });
    }
}