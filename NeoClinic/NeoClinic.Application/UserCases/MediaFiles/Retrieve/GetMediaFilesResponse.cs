using NeoClinic.Domain.Enums;
using Telegram.Bot.Types;

namespace NeoClinic.Application.UserCases.MediaFiles.Retrieve;

public record GetMediaFilesResponse(
    Guid Id,
    string? FileDescriptionUz,
    string? FileDescriptionRu,
    string? AltTextUz,
    string? AltTextRu,
    string FileUrl,
    string? ContentType,
    MediaType Type);
