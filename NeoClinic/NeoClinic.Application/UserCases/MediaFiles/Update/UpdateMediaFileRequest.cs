using MediatR;
using Microsoft.AspNetCore.Http;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Update;

public class UpdateMediaFileRequest : IRequest<bool>
{
    public Guid Id { get; set; }
    public string? FileDescriptionUz { get; set; }
    public string? FileDescriptionRu { get; set; }
    public string? AltTextUz { get; set; }
    public string? AltTextRu { get; set; }
    public MediaType? Type { get; set; }
    public IFormFile? File { get; set; }
    public IFormFile? Thumbnail { get; set; }
}
