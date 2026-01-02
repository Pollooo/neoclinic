using FluentValidation;

namespace NeoClinic.Application.UserCases.Doctors.DeleteDoctor;

public class DeleteDoctorRequestValidator : AbstractValidator<DeleteDoctorRequest>
{
    public DeleteDoctorRequestValidator()
    {
        RuleFor(r => r.DoctorId).NotEmpty().WithMessage("Doctor Id is required to delete a doctor");
    }
}
