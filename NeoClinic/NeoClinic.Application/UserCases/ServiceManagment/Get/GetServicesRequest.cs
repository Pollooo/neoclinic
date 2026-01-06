using MediatR;

namespace NeoClinic.Application.UserCases.Services.Get;

public record GetServicesRequest(Guid? ServiceId) : IRequest<List<GetServicesResponse>>;
