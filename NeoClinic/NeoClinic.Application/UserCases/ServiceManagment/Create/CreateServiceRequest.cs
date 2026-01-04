using MediatR;

namespace NeoClinic.Application.UserCases.Services.Create;

public record CreateServiceRequest(
    string NameUz,
    string? DescriptionUz,
    string NameRu,
    string? DescriptionRu,
    decimal? Price)
    : IRequest<bool>;
