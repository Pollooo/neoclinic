using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.Appointments.DeleteAppointment;

public class DeleteAppointmentRequestHandler(
    IApplicationDbContext context) : IRequestHandler<DeleteAppointmentRequest, bool>
{
    public async Task<bool> Handle(DeleteAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = await context.Appointments
            .FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);

        if (appointment is null)
            return false;

        context.Appointments.Remove(appointment);

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
