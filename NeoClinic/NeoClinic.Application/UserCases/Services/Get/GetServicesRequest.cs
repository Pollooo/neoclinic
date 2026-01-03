using MediatR;

namespace NeoClinic.Application.UserCases.Services.Get;

public record GetServicesRequest() : IRequest<List<GetServicesResponse>>;
