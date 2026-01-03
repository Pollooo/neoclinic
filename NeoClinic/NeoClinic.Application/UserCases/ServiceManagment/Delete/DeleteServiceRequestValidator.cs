using FluentValidation;

namespace NeoClinic.Application.UserCases.Services.Delete;

public class DeleteServiceRequestValidator : AbstractValidator<DeleteServiceRequest>
{
    public DeleteServiceRequestValidator()
    {
        RuleFor(s => s.ServiceId).NotEmpty().WithMessage("Service Id is required to delete a service");
    }
}
