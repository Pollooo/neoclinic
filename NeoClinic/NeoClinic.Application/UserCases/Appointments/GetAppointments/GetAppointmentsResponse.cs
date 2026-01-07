using NeoClinic.Application.UserCases.Services.Get;

namespace NeoClinic.Application.UserCases.Appointments.GetAppointments;

public record GetAppointmentsResponse(
    Guid AppointmentId,
    string PatientName,
    string PhoneNumber,
    string? Email,
    string? Message,
    DateTime AppointmentDate,
    GetServicesResponse GetServicesResponse,
    DateTime CreatedAt);
