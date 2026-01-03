using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Retrieve;

public record GetMediaFilesResponse(
    Guid Id,
    string FileName,
    string FileDescription,
    string FileUrl,
    string? ContentType,
    MediaType Type);
