using MediatR;

namespace NeoClinic.Application.UserCases.ContactMessageManagment.GetContactMessage;

public record GetContactMessageRequest(Guid? ContactId) : IRequest<GetContactMessageResponse?>;
