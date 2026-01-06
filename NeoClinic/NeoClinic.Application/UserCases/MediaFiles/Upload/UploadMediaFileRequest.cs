using MediatR;
using Microsoft.AspNetCore.Http;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Upload;

public record UploadMediaFileRequest(
    string? FileDescriptionUz,
    string? FileDescriptionRu,
    string? AltTextUz,
    string? AltTextRu,
    MediaType Type,
    IFormFile File)
    : IRequest<bool>;
