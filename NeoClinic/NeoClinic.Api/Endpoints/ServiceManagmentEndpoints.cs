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

        app.MapDelete($"{GroupName}/delete/{{serviceId:guid}}", DeleteServiceAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapGet($"{GroupName}/get/{{serviceId:guid}}", GetServicesAsync);
    }

    private static async Task<IResult> CreateServiceAsync(
        CreateServiceRequest request,
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

    private static async Task<IResult> GetServicesAsync(Guid? serviceId, ISender sender)
    {
        var result = await sender.Send(new GetServicesRequest(serviceId));
        return Results.Ok(result);
    }
}
