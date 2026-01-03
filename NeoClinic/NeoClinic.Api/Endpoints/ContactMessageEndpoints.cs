using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.ContactMessageManagment.CreateContactMessage;
using NeoClinic.Application.UserCases.ContactMessageManagment.GetContactMessage;
using NeoClinic.Application.UserCases.ContactMessageManagment.UpdateContactMessage;

namespace NeoClinic.Api.Endpoints;

public static class ContactMessageEndpoints
{
    private const string GroupName = "api/contact-message";

    public static void MapContactMessageEndpoints(this WebApplication app)
    {
        app.MapPost($"{GroupName}/create", CreateContactMessageAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapPut($"{GroupName}/update", UpdateContactMessageAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapGet($"{GroupName}/get", GetContactMessageAsync);
    }

    private static async Task<IResult> CreateContactMessageAsync(
        [FromBody] CreateContactMessageRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> UpdateContactMessageAsync(
        [FromBody] UpdateContactMessageRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> GetContactMessageAsync(
        ISender sender)
    {
        var result = await sender.Send(new GetContactMessageRequest());
        return result is not null ? Results.Ok(result) : Results.NotFound();
    }
}
