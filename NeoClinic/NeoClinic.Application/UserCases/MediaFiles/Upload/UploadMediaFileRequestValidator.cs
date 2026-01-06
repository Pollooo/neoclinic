using FluentValidation;

namespace NeoClinic.Application.UserCases.MediaFiles.Upload;

public class UploadMediaFileRequestValidator : AbstractValidator<UploadMediaFileRequest>
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".mp4", ".mov", ".avi",
    };

    public UploadMediaFileRequestValidator()
    {
        RuleFor(x => x.FileDescriptionUz)
            .NotEmpty().WithMessage("Fayl tavsifi majburiy.")
            .MaximumLength(1000).WithMessage("Fayl tavsifi 1000 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.AltTextUz)
            .MaximumLength(300).WithMessage("Alt matn 300 ta belgidan oshmasligi kerak.");

        RuleFor(x => x.FileDescriptionRu)
            .NotEmpty().WithMessage("Описание файла обязательно.")
            .MaximumLength(1000).WithMessage("Описание файла не должно превышать 1000 символов.");

        RuleFor(x => x.AltTextRu)
            .MaximumLength(300).WithMessage("Alt-текст не должен превышать 300 символов.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid media type.");

        RuleFor(x => x.File)
            .NotNull().WithMessage("File is required.")
            .Must(f =>
            {
                var extension = Path.GetExtension(f.FileName);
                return AllowedExtensions.Contains(extension);
            })
            .WithMessage("File must be a valid image (.jpg, .jpeg, .png, .webp) or video (.mp4, .mov, .avi)");

        RuleFor(x => x.File)
            .Must(f => f.Length <= 50 * 1024 * 1024) // 50 MB
            .WithMessage("File size must not exceed 50 MB.");
    }
}