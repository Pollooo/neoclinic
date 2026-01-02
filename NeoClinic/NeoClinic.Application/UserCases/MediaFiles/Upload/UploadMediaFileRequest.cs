using MediatR;
using Microsoft.AspNetCore.Http;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Upload;

public record UploadMediaFileRequest(
    string FileName,
    string FileDescription,
    string? AltText,
    MediaType Type,
    IFormFile File)
    : IRequest<bool>;
