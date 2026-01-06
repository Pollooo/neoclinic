using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.ContactMessageManagment.GetContactMessage;

public class GetContactMessageRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetContactMessageRequest, GetContactMessageResponse?>
{
    public async Task<GetContactMessageResponse?> Handle(GetContactMessageRequest request, CancellationToken cancellationToken)
    {
        return await context.ContactMessages
            .Select(cm => new GetContactMessageResponse(
            cm.Name,
            cm.Email,
            cm.PhoneNumber,
            cm.AdditionalPhoneNumber,
            cm.TelegramChatUrl,
            cm.TelegramUrl,
            cm.InstagramUrl,
            cm.FacebookUrl,
            cm.LocationUrl,
            cm.AboutClinicUz,
            cm.AboutClinicRu)
        ).FirstOrDefaultAsync(cancellationToken);
    }
}
