using NeoClinic.Application.UserCases.AuthManagment.LogIn;

namespace NeoClinic.Application.Common.Interfaces;

public interface ITokenService
{
    LogInResponse GenerateAccessToken(
        Guid userId,
        string username,
        bool isAdmin
    );
}
