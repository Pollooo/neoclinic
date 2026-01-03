using MediatR;

namespace NeoClinic.Application.UserCases.Services.Create;

public record CreateServiceRequest(
    string Name,
    string? Description,
    decimal? Price)
    : IRequest<bool>;
