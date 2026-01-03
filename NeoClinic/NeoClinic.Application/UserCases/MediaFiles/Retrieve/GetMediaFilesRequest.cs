using MediatR;

namespace NeoClinic.Application.UserCases.MediaFiles.Retrieve;

public record GetMediaFilesRequest() : IRequest<List<GetMediaFilesResponse>>;
