using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.MediaFiles.Retrieve;

public class GetMediaFilesRequestHandler(
    IApplicationDbContext context) : IRequestHandler<GetMediaFilesRequest, List<GetMediaFilesResponse>>
{
    public async Task<List<GetMediaFilesResponse>> Handle(GetMediaFilesRequest request, CancellationToken cancellationToken)
    {
        return await context.MediaFiles
            .Where(f => !request.MediaFileId.HasValue || request.MediaFileId == f.Id).Select(
            f => new GetMediaFilesResponse(
                f.Id,
                f.FileDescriptionUz,
                f.FileDescriptionRu,
                f.AltTextUz,
                f.AltTextRu,
                f.FileUrl,
                f.ContentType,
                f.Type))
            .ToListAsync(cancellationToken);
    }
}
