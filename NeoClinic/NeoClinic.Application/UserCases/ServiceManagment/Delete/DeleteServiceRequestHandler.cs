using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.Services.Delete;

public class DeleteServiceRequestHandler(
    IApplicationDbContext context) : IRequestHandler<DeleteServiceRequest, bool>
{
    public async Task<bool> Handle(DeleteServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await context.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken);
        if (service is null)
            return false;

        var appointments = await context.Appointments
            .Where(a => a.ServiceId == request.ServiceId)
            .ToListAsync(cancellationToken);

        foreach (var appointment in appointments)
        {
            appointment.ServiceId = null;
        }

        context.Services.Remove(service);

        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;
        return false;
    }
}
