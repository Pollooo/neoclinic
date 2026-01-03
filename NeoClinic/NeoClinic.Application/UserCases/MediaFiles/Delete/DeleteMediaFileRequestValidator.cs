using FluentValidation;

namespace NeoClinic.Application.UserCases.MediaFiles.Delete;

public class DeleteMediaFileRequestValidator : AbstractValidator<DeleteMediaFileRequest>
{
    public DeleteMediaFileRequestValidator()
    {
        RuleFor(f => f.FileId).NotEmpty().WithMessage("File Id is required to delete a file");
    }
}
