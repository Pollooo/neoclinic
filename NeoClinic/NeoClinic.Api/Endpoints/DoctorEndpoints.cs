using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.Doctors.CreateDoctor;
using NeoClinic.Application.UserCases.Doctors.DeleteDoctor;
using NeoClinic.Application.UserCases.Doctors.GetDoctors;
using NeoClinic.Application.UserCases.Doctors.UpdateDoctor;
using NeoClinic.Application.UserCases.Doctors.UpdateDoctorPhoto;

namespace NeoClinic.Api.Endpoints;

public static class DoctorEndpoints
{
    private readonly static string GroupName = "api/doctors";
    public static void MapDoctorEndpoints(this WebApplication app)
    {
        app.MapPost($"{GroupName}/create", CreateDoctorAsync)
             .DisableAntiforgery()
             .RequireAuthorization("AdminPolicy");
        app.MapDelete($"{GroupName}/delete/{{doctorId:guid}}", DeleteDoctorAsync)
            .RequireAuthorization("AdminPolicy");
        app.MapPut($"{GroupName}/update", UpdateDoctorAsync)
            .RequireAuthorization("AdminPolicy");
        app.MapPut($"{GroupName}/update-photo", UpdateDoctorPhotoAsync)
            .DisableAntiforgery()
            .RequireAuthorization("AdminPolicy");
        app.MapGet($"{GroupName}/get", GetDoctorsAsync);
    }

    private static async Task<IResult> CreateDoctorAsync(
       [FromForm] CreateDoctorRequest request,
       ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> DeleteDoctorAsync(
        Guid doctorId,
        ISender sender)
    {
        var request = new DeleteDoctorRequest(doctorId);
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> UpdateDoctorAsync(
        UpdateDoctorRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> UpdateDoctorPhotoAsync(
        [FromForm] UpdateDoctorPhotoRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> GetDoctorsAsync(Guid? doctorId, ISender sender)
    {
        var result = await sender.Send(new GetDoctorsRequest(doctorId));
        return Results.Ok(result);
    }
}
