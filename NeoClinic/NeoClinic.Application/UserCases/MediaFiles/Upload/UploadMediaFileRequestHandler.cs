using MediatR;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;

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
        };

        await context.MediaFiles.AddAsync(document, cancellationToken);

        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        return false;
    }
}
