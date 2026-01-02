using FluentValidation;

namespace NeoClinic.Application.UserCases.Appointments.CreateAppointment;

public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientName)
                .NotEmpty().WithMessage("Patient name is required")
                .MaximumLength(100).WithMessage("Patient name must not exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.Email)
            .MaximumLength(100).WithMessage("Email must not exceed 100 characters")
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Email is not valid");

        RuleFor(x => x.Message)
            .MaximumLength(500).WithMessage("Message must not exceed 500 characters");

        RuleFor(x => x.ServiceId)
            .NotEmpty().WithMessage("Service must be selected");

        RuleFor(x => x.AppointmentDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future");
    }
}
