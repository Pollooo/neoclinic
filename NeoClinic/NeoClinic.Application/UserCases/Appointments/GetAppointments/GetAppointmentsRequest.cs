using MediatR;

namespace NeoClinic.Application.UserCases.Appointments.GetAppointments;

public record GetAppointmentsRequest(DateOnly? StartDate, DateOnly? EndDate)
    : IRequest<List<GetAppointmentsResponse>?>;
