using MediatR;

namespace NeoClinic.Application.UserCases.ContactMessageManagment.CreateContactMessage;

public record CreateContactMessageRequest(
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
