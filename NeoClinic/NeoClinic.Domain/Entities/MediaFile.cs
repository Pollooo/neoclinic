using NeoClinic.Domain.Enums;

namespace NeoClinic.Domain.Entities;

public class MediaFile
{
    public Guid Id { get; set; }

    public string FileName { get; set; } = null!;
    public string FileUrl { get; set; } = null!; // S3 / Railway / Cloud URL

    public MediaType Type { get; set; } // Image or Video

    public string? AltText { get; set; } // for UI / SEO

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
