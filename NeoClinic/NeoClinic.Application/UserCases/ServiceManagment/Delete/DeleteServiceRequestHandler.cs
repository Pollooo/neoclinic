using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace NeoClinic.Application.UserCases.Services.Delete;

public class DeleteServiceRequestHandler(
    IApplicationDbContext context) : IRequestHandler<DeleteServiceRequest, bool>
{
    public async Task<bool> Handle(DeleteServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await context.Services.FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken);
        if (service is null)
            return false;

        // Check if service is linked to any appointments
        var hasAppointments = await context.Appointments
            .AnyAsync(a => a.ServiceId == request.ServiceId, cancellationToken);
        
        if (hasAppointments)
            throw new InvalidOperationException("Cannot delete service that has appointments linked to it");

        context.Services.Remove(service);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;
        return false;
    }
}
