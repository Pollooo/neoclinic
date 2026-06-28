using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.MediaFiles.Delete;

public class DeleteMediaFileRequestHandler(
    IApplicationDbContext context,
    IStorageService storageService) : IRequestHandler<DeleteMediaFileRequest, bool>
{
    public async Task<bool> Handle(DeleteMediaFileRequest request, CancellationToken cancellationToken)
    {
        var doc = await context.MediaFiles.FirstOrDefaultAsync(f => f.Id == request.FileId, cancellationToken);
        if (doc is null)
            return false;

        try
        {
            await storageService.DeleteFileAsync(doc.BlobName);
        }
        catch
        {
            // File might be in an old/unavailable storage provider — skip
        }

        if (!string.IsNullOrWhiteSpace(doc.ThumbnailBlobName))
        {
            try
            {
                await storageService.DeleteFileAsync(doc.ThumbnailBlobName);
            }
            catch
            {
                // Thumbnail might be in an old/unavailable storage provider — skip
            }
        }

        context.MediaFiles.Remove(doc);
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
}
