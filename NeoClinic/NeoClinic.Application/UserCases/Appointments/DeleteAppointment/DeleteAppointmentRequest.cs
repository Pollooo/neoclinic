using MediatR;

namespace NeoClinic.Application.UserCases.Appointments.DeleteAppointment;

public record DeleteAppointmentRequest(Guid AppointmentId) : IRequest<bool>;
