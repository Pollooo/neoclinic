using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Application.UserCases.AuthManagment.LogIn;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NeoClinic.Application.Common.Services;

public class TokenService(IConfiguration _configuration) : ITokenService
{
    public LogInResponse GenerateAccessToken(Guid userId, string username, bool isAdmin)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, username),
            new("isAdmin", isAdmin.ToString()),
            new(ClaimTypes.Role, "Admin"),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Get expiration from config
        var minutes = Convert.ToDouble(_configuration["Jwt:AccessTokenMinutes"]);
        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new LogInResponse(new JwtSecurityTokenHandler().WriteToken(token), expires);
    }
}
