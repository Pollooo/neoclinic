using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.Services.Create;
using NeoClinic.Application.UserCases.Services.Delete;
using NeoClinic.Application.UserCases.Services.Get;

namespace NeoClinic.Api.Endpoints;

public static class ServiceManagmentEndpoints
{
    private const string GroupName = "api/services";

    public static void MapServiceEndpoints(this WebApplication app)
    {
        app.MapPost($"{GroupName}/create", CreateServiceAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapDelete($"{GroupName}/{{serviceId:guid}}", DeleteServiceAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapGet($"{GroupName}/get", GetServicesAsync);
    }

    private static async Task<IResult> CreateServiceAsync(
        [FromForm] CreateServiceRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> DeleteServiceAsync(
        Guid serviceId,
        ISender sender)
    {
        var request = new DeleteServiceRequest(serviceId);
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> GetServicesAsync(ISender sender)
    {
        var result = await sender.Send(new GetServicesRequest());
        return Results.Ok(result);
    }
}
