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

        context.Services.Remove(service);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;
        return false;
    }
}
