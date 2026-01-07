using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.Appointments.CreateAppointment;
using NeoClinic.Application.UserCases.Appointments.GetAppointments;

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

        app.MapGet($"{GroupName}/get", GetAppointmentsAsync)
           .RequireAuthorization("AdminPolicy")
           .Produces<List<GetAppointmentsResponse>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> CreateAppointmentAsync(
        [FromBody] CreateAppointmentRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> GetAppointmentsAsync(
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        ISender sender)
    {
        var result = await sender.Send(new GetAppointmentsRequest(startDate, endDate));
        return Results.Ok(result);
    }
}
