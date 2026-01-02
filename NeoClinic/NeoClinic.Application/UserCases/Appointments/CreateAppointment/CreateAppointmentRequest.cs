using MediatR;

namespace NeoClinic.Application.UserCases.Appointments.CreateAppointment;

public record CreateAppointmentRequest(
    string PatientName,
    string PhoneNumber,
    string? Email,
    string? Message,
    Guid ServiceId,
    DateTime AppointmentDate)
    : IRequest<bool>;
