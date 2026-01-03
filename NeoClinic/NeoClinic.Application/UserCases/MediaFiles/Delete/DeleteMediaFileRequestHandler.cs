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

        await storageService.DeleteFileAsync(doc.BlobName);
        context.MediaFiles.Remove(doc);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return true;

        return false;
    }
}
