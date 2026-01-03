namespace NeoClinic.Application.UserCases.Services.Get;

public record GetServicesResponse(
    Guid ServiceId, string Name, string? Description, decimal? Price);
