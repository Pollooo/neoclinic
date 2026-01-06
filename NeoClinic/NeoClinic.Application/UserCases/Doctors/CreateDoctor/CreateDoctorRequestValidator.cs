using FluentValidation;

namespace NeoClinic.Application.UserCases.Doctors.CreateDoctor;

public class CreateDoctorRequestValidator : AbstractValidator<CreateDoctorRequest>
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public CreateDoctorRequestValidator()
    {
        RuleFor(x => x.FullNameRu).NotEmpty();
        RuleFor(x => x.SpecialtyRu).NotEmpty();
        RuleFor(x => x.BioRu).NotEmpty();

        RuleFor(x => x.FullNameUz).NotEmpty();
        RuleFor(x => x.SpecialtyUz).NotEmpty();
        RuleFor(x => x.BioUz).NotEmpty();

        RuleFor(x => x.Photo)
            .NotNull()
            .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName)))
            .WithMessage("Photo is required and must be a valid image (.jpg, .jpeg, .png, .webp)");
    }
}
