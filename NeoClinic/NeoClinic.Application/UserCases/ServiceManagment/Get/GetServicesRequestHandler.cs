using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.Services.Get;

public class GetServicesRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetServicesRequest, List<GetServicesResponse>>
{
    public async Task<List<GetServicesResponse>> Handle(GetServicesRequest request, CancellationToken cancellationToken)
    {
        return await context.Services
            .Where(f => !request.ServiceId.HasValue || request.ServiceId == f.Id).Select(
            s => new GetServicesResponse(
                s.Id,
                s.NameUz,
                s.DescriptionUz,
                s.NameRu,
                s.DescriptionRu,
                s.Price))
            .ToListAsync(cancellationToken);
    }
}
