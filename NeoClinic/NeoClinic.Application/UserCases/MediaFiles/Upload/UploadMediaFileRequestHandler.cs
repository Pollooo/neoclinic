using MediatR;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using System.Text.Json;

namespace NeoClinic.Application.UserCases.MediaFiles.Upload;

public class UploadMediaFileRequestHandler(
    IApplicationDbContext context,
    IStorageService storageService,
    ITelegramBotService botService)
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
            FileDescription = request.FileDescription,
            FileUrl = fileUrl,
            ContainerName = "neo-clinic-docs",
            ContentType = request.File.ContentType,
            IsDoctor = true,
            Type = request.Type,
            AltText = request.AltText,
        };

        await context.MediaFiles.AddAsync(document, cancellationToken);

        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        await botService.NotifyAboutErrorAsync(JsonSerializer.Serialize(request), "UploadMediaFileRequestHandler");
        return false;
    }
}
