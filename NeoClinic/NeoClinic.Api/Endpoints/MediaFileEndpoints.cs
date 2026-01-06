using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.UserCases.MediaFiles.Delete;
using NeoClinic.Application.UserCases.MediaFiles.Retrieve;
using NeoClinic.Application.UserCases.MediaFiles.Upload;

namespace NeoClinic.Api.Endpoints;

public static class MediaFileEndpoints
{
    private const string GroupName = "api/media-files";

    public static void MapMediaFileEndpoints(this WebApplication app)
    {
        app.MapPost($"{GroupName}/upload", UploadMediaFileAsync)
           .Accepts<IFormFile>("multipart/form-data")
           .DisableAntiforgery()
           .RequireAuthorization("AdminPolicy");

        app.MapDelete($"{GroupName}/delete/{{fileId:guid}}", DeleteMediaFileAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapGet($"{GroupName}/get/{{fileId:guid}}", GetMediaFilesAsync);
    }

    private static async Task<IResult> UploadMediaFileAsync(
        [FromForm] UploadMediaFileRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> DeleteMediaFileAsync(
        Guid fileId,
        ISender sender)
    {
        var request = new DeleteMediaFileRequest(fileId);
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> GetMediaFilesAsync(Guid? fileId, ISender sender)
    {
        var result = await sender.Send(new GetMediaFilesRequest(fileId));
        return Results.Ok(result);
    }
}