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
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Specialty).NotEmpty();
        RuleFor(x => x.Bio).NotEmpty();

        // Photo is now mandatory
        RuleFor(x => x.Photo)
            .NotNull()
            .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName)))
            .WithMessage("Photo is required and must be a valid image (.jpg, .jpeg, .png, .webp)");
    }
}
