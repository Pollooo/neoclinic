using FluentValidation;

namespace NeoClinic.Application.UserCases.Appointments.DeleteAppointment;

public class DeleteAppointmentRequestValidator : AbstractValidator<DeleteAppointmentRequest>
{
    public DeleteAppointmentRequestValidator()
    {
        RuleFor(r => r.AppointmentId)
            .NotEmpty().WithMessage("Appointment Id is required to delete an appointment.");
    }
}
