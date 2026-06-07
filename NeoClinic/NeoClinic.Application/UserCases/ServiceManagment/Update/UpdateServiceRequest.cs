using MediatR;

namespace NeoClinic.Application.UserCases.Services.Update;

public record UpdateServiceRequest(
    Guid Id,
    string NameUz,
    string? DescriptionUz,
    string NameRu,
    string? DescriptionRu,
    decimal? Price)
    : IRequest<bool>;
