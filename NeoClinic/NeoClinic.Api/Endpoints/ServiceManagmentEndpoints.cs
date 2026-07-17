using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.Services.Create;
using NeoClinic.Application.UserCases.Services.Delete;
using NeoClinic.Application.UserCases.Services.Get;
using NeoClinic.Application.UserCases.Services.Update;

namespace NeoClinic.Api.Endpoints;

public static class ServiceManagmentEndpoints
{
    private const string GroupName = "api/services";

    public static void MapServiceEndpoints(this WebApplication app)
    {
        app.MapPost($"{GroupName}/create", CreateServiceAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapPut($"{GroupName}/update", UpdateServiceAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapDelete($"{GroupName}/delete/{{serviceId:guid}}", DeleteServiceAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapGet($"{GroupName}/get", GetServicesAsync);
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
        try
        {
            var request = new DeleteServiceRequest(serviceId);
            var result = await sender.Send(request);
            return result ? Results.Ok(true) : Results.BadRequest(false);
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> GetServicesAsync(Guid? serviceId, ISender sender)
    {
        var result = await sender.Send(new GetServicesRequest(serviceId));
        return Results.Ok(result);
    }

    private static async Task<IResult> UpdateServiceAsync(
        UpdateServiceRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.NotFound(false);
    }
}