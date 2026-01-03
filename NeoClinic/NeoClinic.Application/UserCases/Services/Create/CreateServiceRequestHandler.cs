using MediatR;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using System.Text.Json;

namespace NeoClinic.Application.UserCases.Services.Create;

public class CreateServiceRequestHandler(
    IApplicationDbContext context,
    ITelegramBotService botService) : IRequestHandler<CreateServiceRequest, bool>
{
    public async Task<bool> Handle(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var service = new Service()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
        };

        await context.Services.AddAsync(service, cancellationToken);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        await botService.NotifyAboutErrorAsync(JsonSerializer.Serialize(request), "CreateServiceRequestHandler");
        return false;
    }
}
