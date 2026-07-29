using MediatR;
using Microsoft.AspNetCore.Http;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Upload;

public class UploadMediaFileRequest : IRequest<bool>
{
    public string? FileDescriptionUz { get; set; }
    public string? FileDescriptionRu { get; set; }
    public string? AltTextUz { get; set; }
    public string? AltTextRu { get; set; }
    public MediaType Type { get; set; }
    public IFormFile File { get; set; } = null!;
    public IFormFile? Thumbnail { get; set; }
}