using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.AuthManagment.LogIn;

public class LogInRequestHandler(
    IApplicationDbContext context,
    ITokenService tokenService)
    : IRequestHandler<LogInRequest, LogInResponse?>
{
    public async Task<LogInResponse?> Handle(LogInRequest request, CancellationToken cancellationToken)
    {
        var admin = await context.Admins.FirstOrDefaultAsync(a => a.Username == request.Username, cancellationToken);
        if (admin is null || admin.PasswordHash != request.Password)
            return null;

        var logInResponse = tokenService.GenerateAccessToken(admin.Id, admin.Username, true);
        return logInResponse;
    }
}
