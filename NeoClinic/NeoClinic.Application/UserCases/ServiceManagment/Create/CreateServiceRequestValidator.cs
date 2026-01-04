using FluentValidation;

namespace NeoClinic.Application.UserCases.Services.Create;

public class CreateServiceRequestValidator : AbstractValidator<CreateServiceRequest>
{
    public CreateServiceRequestValidator()
    {
        RuleFor(x => x.NameUz)
            .NotEmpty().WithMessage("Service name (UZ) is required.")
            .MaximumLength(100).WithMessage("Service name (UZ) must not exceed 100 characters.");

        RuleFor(x => x.DescriptionUz)
            .MaximumLength(500).WithMessage("Description (UZ) must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.DescriptionUz));

        RuleFor(x => x.NameRu)
            .NotEmpty().WithMessage("Название услуги (RU) обязательно.")
            .MaximumLength(100).WithMessage("Название услуги (RU) не должно превышать 100 символов.");

        RuleFor(x => x.DescriptionRu)
            .MaximumLength(500).WithMessage("Описание (RU) не должно превышать 500 символов.")
            .When(x => !string.IsNullOrEmpty(x.DescriptionRu));

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be >= 0")
            .When(x => x.Price.HasValue);
    }
}
