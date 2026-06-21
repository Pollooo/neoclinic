using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.Services.Update;

public class UpdateServiceRequestHandler(
    IApplicationDbContext context) : IRequestHandler<UpdateServiceRequest, bool>
{
    public async Task<bool> Handle(UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await context.Services
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (service is null)
            return false;

        service.NameUz = request.NameUz;
        service.DescriptionUz = request.DescriptionUz;
        service.NameRu = request.NameRu;
        service.DescriptionRu = request.DescriptionRu;
        service.Price = request.Price;

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
