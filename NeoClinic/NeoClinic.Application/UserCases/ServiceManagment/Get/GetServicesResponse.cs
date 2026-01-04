namespace NeoClinic.Application.UserCases.Services.Get;

public record GetServicesResponse(
    Guid ServiceId, string NameUz, string? DescriptionUz, string NameRu, string? DescriptionRu, decimal? Price);
