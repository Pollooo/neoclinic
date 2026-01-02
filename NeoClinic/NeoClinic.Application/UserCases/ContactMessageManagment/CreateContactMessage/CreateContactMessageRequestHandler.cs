using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using System.Text.Json;

namespace NeoClinic.Application.UserCases.ContactMessageManagment.CreateContactMessage;

public class CreateContactMessageRequestHandler(
    IApplicationDbContext context,
    ITelegramBotService botService)
    : IRequestHandler<CreateContactMessageRequest, bool>
{
    public async Task<bool> Handle(CreateContactMessageRequest request, CancellationToken cancellationToken)
    {
        var contactMessage = await context.ContactMessages.FirstOrDefaultAsync(cancellationToken);
        if (contactMessage is not null)
            return false;

        var contactMessageEntity = new Domain.Entities.ContactMessage
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            AdditionalPhoneNumber = request.AdditionalPhoneNumber,
            TelegramChatUrl = request.TelegramChatUrl,
            TelegramUrl = request.TelegramUrl,
            InstagramUrl = request.InstagramUrl,
            FacebookUrl = request.FacebookUrl,
            LocationUrl = request.LocationUrl,
            AboutClinic = request.AboutClinic,
        };

        await context.ContactMessages.AddAsync(contactMessageEntity, cancellationToken);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        await botService.NotifyAboutErrorAsync(JsonSerializer.Serialize(request), "CreateContactMessageRequestHandler");
        return false;
    }
}
