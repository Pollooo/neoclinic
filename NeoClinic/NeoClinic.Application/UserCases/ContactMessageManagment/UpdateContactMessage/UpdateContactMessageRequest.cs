using MediatR;

namespace NeoClinic.Application.UserCases.ContactMessageManagment.UpdateContactMessage;

public record UpdateContactMessageRequest(
    string? Name,
    string? Email,
    string? PhoneNumber,
    string? AdditionalPhoneNumber,
    string? TelegramChatUrl,
    string? TelegramUrl,
    string? InstagramUrl,
    string? FacebookUrl,
    string? LocationUrl,
    string? AboutClinicUz,
    string? AboutClinicRu)
    : IRequest<bool>;
