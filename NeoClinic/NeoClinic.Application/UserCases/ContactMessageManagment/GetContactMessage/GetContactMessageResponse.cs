namespace NeoClinic.Application.UserCases.ContactMessageManagment.GetContactMessage;

public record GetContactMessageResponse(
    Guid  contactId,
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
    string? AboutClinicRu);
