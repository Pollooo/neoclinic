using FluentValidation;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctor;

public class UpdateDoctorRequestValidator : AbstractValidator<UpdateDoctorRequest>
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public UpdateDoctorRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Specialty).NotEmpty();
        RuleFor(x => x.Bio).NotEmpty();
        RuleFor(x => x.DoctorId).NotEmpty();

        // Photo is now mandatory
        RuleFor(x => x.Photo)
            .NotNull()
            .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName)))
            .WithMessage("Photo is required and must be a valid image (.jpg, .jpeg, .png, .webp)");
    }
}
