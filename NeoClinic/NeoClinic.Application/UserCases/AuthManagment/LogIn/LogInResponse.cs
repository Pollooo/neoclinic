namespace NeoClinic.Application.UserCases.AuthManagment.LogIn;

public record LogInResponse(string AccessToken, DateTime ExpiresAt);
