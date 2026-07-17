namespace NeoClinic.Domain.Entities;

public class TelegramFileMap
{
    public string BlobName { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
