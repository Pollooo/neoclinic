using MediatR;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Upload;

public class UploadMediaFileRequestHandler(
    IApplicationDbContext context,
    IStorageService storageService)
    : IRequestHandler<UploadMediaFileRequest, bool>
{
    public async Task<bool> Handle(UploadMediaFileRequest request, CancellationToken cancellationToken)
    {
        var fileName = request.File.FileName;
        var blobName = storageService.GenerateBlobName(request.Type, fileName);
        var fileUrl = await storageService.UploadFileAsync(blobName, request.File.OpenReadStream());

        string? thumbnailUrl = null;
        string? thumbnailBlobName = null;

        if (request.Thumbnail is not null)
        {
            var thumbExt = Path.GetExtension(request.Thumbnail.FileName);
            var thumbName = $"{Path.GetFileNameWithoutExtension(blobName)}{thumbExt}";
            thumbnailBlobName = storageService.GenerateBlobName(MediaType.Image, thumbName);
            thumbnailUrl = await storageService.UploadFileAsync(thumbnailBlobName, request.Thumbnail.OpenReadStream());
        }

        var document = new MediaFile()
        {
            FileName = fileName,
            BlobName = blobName,
            FileSizeInBytes = request.File.Length,
            FileDescriptionRu = request.FileDescriptionRu,
            FileDescriptionUz = request.FileDescriptionUz,
            FileUrl = fileUrl,
            ContainerName = "neo-clinic-docs",
            ContentType = request.File.ContentType,
            IsDoctor = false,
            Type = request.Type,
            AltTextUz = request.AltTextUz,
            AltTextRu = request.AltTextRu,
            ThumbnailUrl = thumbnailUrl,
            ThumbnailBlobName = thumbnailBlobName,
        };

        await context.MediaFiles.AddAsync(document, cancellationToken);

        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        return false;
    }
}
