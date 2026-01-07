using MediatR;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Application.UserCases.Services.Create;

public class CreateServiceRequestHandler(
    IApplicationDbContext context) : IRequestHandler<CreateServiceRequest, bool>
{
    public async Task<bool> Handle(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var service = new Service()
        {
            NameUz = request.NameUz,
            DescriptionUz = request.DescriptionUz,
            NameRu = request.NameRu,
            DescriptionRu = request.DescriptionRu,
            Price = request.Price,
        };

        await context.Services.AddAsync(service, cancellationToken);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        return false;
    }
}
