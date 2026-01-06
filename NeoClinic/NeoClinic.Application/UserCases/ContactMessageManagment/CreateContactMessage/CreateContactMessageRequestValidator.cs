using FluentValidation;

namespace NeoClinic.Application.UserCases.ContactMessageManagment.CreateContactMessage;

public class CreateContactMessageRequestValidator : AbstractValidator<CreateContactMessageRequest>
{
    public CreateContactMessageRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email format");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20);

        RuleFor(x => x.AdditionalPhoneNumber)
            .MaximumLength(20);

        RuleFor(x => x.TelegramChatUrl)
            .MaximumLength(300);

        RuleFor(x => x.TelegramUrl)
            .MaximumLength(300);

        RuleFor(x => x.InstagramUrl)
            .MaximumLength(300);

        RuleFor(x => x.FacebookUrl)
            .MaximumLength(300);

        RuleFor(x => x.LocationUrl)
            .MaximumLength(300);

        RuleFor(x => x.AboutClinicUz)
            .MaximumLength(2000);

        RuleFor(x => x.AboutClinicRu)
            .MaximumLength(2000);

        RuleFor(x => x)
            .Must(x =>
                !string.IsNullOrWhiteSpace(x.PhoneNumber) ||
                !string.IsNullOrWhiteSpace(x.Email) ||
                !string.IsNullOrWhiteSpace(x.TelegramChatUrl))
            .WithMessage("Provide at least one contact method");
    }
}
