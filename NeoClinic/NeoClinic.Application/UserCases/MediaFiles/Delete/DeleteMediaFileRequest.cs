using MediatR;

namespace NeoClinic.Application.UserCases.MediaFiles.Delete;

public record DeleteMediaFileRequest(Guid FileId) : IRequest<bool>;
