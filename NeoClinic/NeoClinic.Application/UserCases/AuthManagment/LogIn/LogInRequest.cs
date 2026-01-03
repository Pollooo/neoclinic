using MediatR;

namespace NeoClinic.Application.UserCases.AuthManagment.LogIn;

public record LogInRequest(string Username, string Password) : IRequest<LogInResponse?>;
