using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using System.Text.Json;

namespace NeoClinic.Application.UserCases.ContactMessageManagment.UpdateContactMessage;

public class UpdateContactMessageRequestHandler(
    IApplicationDbContext context,
    ITelegramBotService botService)
    : IRequestHandler<UpdateContactMessageRequest, bool>
{
    public async Task<bool> Handle(UpdateContactMessageRequest request, CancellationToken cancellationToken)
    {
        var contactMessage = await context.ContactMessages
            .FirstOrDefaultAsync(cancellationToken);
        if (contactMessage is null)
            return false;

        contactMessage.Name = request.Name ?? contactMessage.Name;
        contactMessage.Email = request.Email ?? contactMessage.Email;
        contactMessage.PhoneNumber = request.PhoneNumber ?? contactMessage.PhoneNumber;
        contactMessage.AdditionalPhoneNumber = request.AdditionalPhoneNumber ?? contactMessage.AdditionalPhoneNumber;
        contactMessage.TelegramChatUrl = request.TelegramChatUrl ?? contactMessage.TelegramChatUrl;
        contactMessage.TelegramUrl = request.TelegramUrl ?? contactMessage.TelegramUrl;
        contactMessage.InstagramUrl = request.InstagramUrl ?? contactMessage.InstagramUrl;
        contactMessage.FacebookUrl = request.FacebookUrl ?? contactMessage.FacebookUrl;
        contactMessage.LocationUrl = request.LocationUrl ?? contactMessage.LocationUrl;
        contactMessage.AboutClinic = request.AboutClinic ?? contactMessage.AboutClinic;

        context.ContactMessages.Update(contactMessage);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        await botService.NotifyAboutErrorAsync(JsonSerializer.Serialize(request), "UpdateContactMessageRequestHandler");
        return false;
    }
}
