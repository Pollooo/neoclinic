using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Update;

public class UpdateMediaFileRequestHandler(
    IApplicationDbContext context,
    IStorageService storageService) : IRequestHandler<UpdateMediaFileRequest, bool>
{
    public async Task<bool> Handle(UpdateMediaFileRequest request, CancellationToken cancellationToken)
    {
        var mediaFile = await context.MediaFiles
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (mediaFile is null)
            return false;

        mediaFile.FileDescriptionUz = request.FileDescriptionUz;
        mediaFile.FileDescriptionRu = request.FileDescriptionRu;
        mediaFile.AltTextUz = request.AltTextUz;
        mediaFile.AltTextRu = request.AltTextRu;

        if (request.File is not null)
        {
            var type = request.Type ?? mediaFile.Type;
            var oldBlobName = mediaFile.BlobName;
            var fileName = request.File.FileName;
            var blobName = storageService.GenerateBlobName(type, fileName);
            var fileUrl = await storageService.UploadFileAsync(blobName, request.File.OpenReadStream());

            mediaFile.FileName = fileName;
            mediaFile.BlobName = blobName;
            mediaFile.FileUrl = fileUrl;
            mediaFile.FileSizeInBytes = request.File.Length;
            mediaFile.ContentType = request.File.ContentType;
            mediaFile.Type = type;

            if (!string.IsNullOrWhiteSpace(oldBlobName))
            {
                try
                {
                    await storageService.DeleteFileAsync(oldBlobName);
                }
                catch
                {
                    // Old file might be in a different storage provider or unavailable — skip
                }
            }
        }

        if (request.Thumbnail is not null)
        {
            var oldThumbBlobName = mediaFile.ThumbnailBlobName;

            var thumbBlobName = storageService.GenerateBlobName(
                MediaType.Image,
                $"{Guid.NewGuid():N}{Path.GetExtension(request.Thumbnail.FileName)}");
            var thumbUrl = await storageService.UploadFileAsync(thumbBlobName, request.Thumbnail.OpenReadStream());

            mediaFile.ThumbnailUrl = thumbUrl;
            mediaFile.ThumbnailBlobName = thumbBlobName;

            if (!string.IsNullOrWhiteSpace(oldThumbBlobName))
            {
                try
                {
                    await storageService.DeleteFileAsync(oldThumbBlobName);
                }
                catch
                {
                    // Old thumbnail might be in a different storage provider or unavailable — skip
                }
            }
        }

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
