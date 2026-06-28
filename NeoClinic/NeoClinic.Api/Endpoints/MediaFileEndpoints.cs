using MediatR;
using Microsoft.AspNetCore.Mvc;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Application.UserCases.MediaFiles.Delete;
using NeoClinic.Application.UserCases.MediaFiles.Retrieve;
using NeoClinic.Application.UserCases.MediaFiles.Update;
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

        app.MapPut($"{GroupName}/update", UpdateMediaFileAsync)
           .Accepts<IFormFile>("multipart/form-data")
           .DisableAntiforgery()
           .RequireAuthorization("AdminPolicy");

        app.MapDelete($"{GroupName}/delete/{{fileId:guid}}", DeleteMediaFileAsync)
           .RequireAuthorization("AdminPolicy");

        app.MapGet($"{GroupName}/get", GetMediaFilesAsync);
        app.MapGet($"{GroupName}/proxy/{{**blobName}}", ProxyMediaFileAsync);
    }

    private static async Task<IResult> UploadMediaFileAsync(
        [FromForm] UploadMediaFileRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.BadRequest(false);
    }

    private static async Task<IResult> UpdateMediaFileAsync(
        [FromForm] UpdateMediaFileRequest request,
        ISender sender)
    {
        var result = await sender.Send(request);
        return result ? Results.Ok(true) : Results.NotFound(false);
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

    private static async Task<IResult> ProxyMediaFileAsync(
        string blobName,
        IStorageService storageService)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            return Results.BadRequest("Blob name is required");

        try
        {
            var (content, contentType) = await storageService.GetFileBytesAsync(blobName);
            return Results.File(content, contentType);
        }
        catch
        {
            return Results.NotFound();
        }
    }

}
