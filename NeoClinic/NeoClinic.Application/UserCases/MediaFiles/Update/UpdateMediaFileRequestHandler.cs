using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

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
                await storageService.DeleteFileAsync(oldBlobName);
        }

        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
