namespace NeoClinic.Domain.Entities;

public class Doctor
{
    public Guid Id { get; set; }
    public string FullNameUz { get; set; } = null!;
    public string FullNameRu { get; set; } = null!;
    public string? SpecialtyUz { get; set; }
    public string? SpecialtyRu { get; set; }
    public string? PhotoUrl { get; set; }
    public string? BioUz { get; set; }
    public string? BioRu { get; set; }
    public string BlobName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
