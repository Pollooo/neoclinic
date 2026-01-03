using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.AuthManagment.LogIn;

namespace NeoClinic.Api.Endpoints;

public static class AuthManagmentEndpoints
{
    private const string GroupName = "api/auth";

    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost($"{GroupName}/login", LogInAsync)
           .Accepts<LogInRequest>("application/json")
           .Produces<LogInResponse>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> LogInAsync(
        [FromBody] LogInRequest request,
        ISender sender)
    {
        var response = await sender.Send(request);
        if (response is null) return Results.Unauthorized();
        return Results.Ok(response);
    }
}

