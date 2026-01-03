using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.Appointments.CreateAppointment;

namespace NeoClinic.Api.Endpoints;

public static class AppointmentEndpoints
{
    private const string GroupName = "api/appointments";

    public static void MapAppointmentEndpoints(this WebApplication app)
    {
        app.MapPost($"{GroupName}/create", CreateAppointmentAsync)
           .Accepts<CreateAppointmentRequest>("application/json")
           .Produces<bool>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateAppointmentAsync(
        [FromBody] CreateAppointmentRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }
}
