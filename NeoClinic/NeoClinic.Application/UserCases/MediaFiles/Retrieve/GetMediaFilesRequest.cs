using MediatR;

namespace NeoClinic.Application.UserCases.MediaFiles.Retrieve;

public record GetMediaFilesRequest(Guid? MediaFileId) : IRequest<List<GetMediaFilesResponse>>;
