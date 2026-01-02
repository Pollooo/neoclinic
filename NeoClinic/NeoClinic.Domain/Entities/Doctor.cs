namespace NeoClinic.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string? Specialty { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Bio { get; set; }
    public string BlobName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
