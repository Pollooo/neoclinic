using NeoClinic.Domain.Enums;

namespace NeoClinic.Domain.Entities;

public class MediaFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public long FileSizeInBytes { get; set; }
    public string? FileDescriptionUz { get; set; } = null!;
    public string? FileDescriptionRu { get; set; } = null!;
    public string ContainerName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = null!; // S3 / Railway / Cloud URL
    public string BlobName { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public string? AltTextUz { get; set; } // for UI / SEO
    public string? AltTextRu { get; set; } // for UI / SEO
    public bool IsDoctor { get; set; } = false;
    public MediaType Type { get; set; } // Image or Video
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
