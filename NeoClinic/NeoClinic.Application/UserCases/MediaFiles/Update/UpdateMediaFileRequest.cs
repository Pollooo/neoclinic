using MediatR;
using Microsoft.AspNetCore.Http;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.MediaFiles.Update;

public record UpdateMediaFileRequest(
    Guid Id,
    string? FileDescriptionUz,
    string? FileDescriptionRu,
    string? AltTextUz,
    string? AltTextRu,
    MediaType? Type,
    IFormFile? File,
    IFormFile? Thumbnail)
    : IRequest<bool>;
