using FluentValidation;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctorPhoto;

public class UpdateDoctorPhotoRequestValidator : AbstractValidator<UpdateDoctorPhotoRequest>
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public UpdateDoctorPhotoRequestValidator()
    {
        RuleFor(x => x.DoctorId).NotEmpty();
        RuleFor(x => x.Photo)
            .NotNull()
            .Must(f => AllowedExtensions.Contains(Path.GetExtension(f.FileName)))
            .WithMessage("Photo is required and must be a valid image (.jpg, .jpeg, .png, .webp)");
    }
}
