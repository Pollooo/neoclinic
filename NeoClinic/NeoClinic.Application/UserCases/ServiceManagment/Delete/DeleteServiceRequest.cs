using MediatR;

namespace NeoClinic.Application.UserCases.Services.Delete;

public record DeleteServiceRequest(Guid ServiceId) : IRequest<bool>;
