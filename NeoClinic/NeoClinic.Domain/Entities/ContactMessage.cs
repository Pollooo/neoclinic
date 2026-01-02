namespace NeoClinic.Domain.Entities;

public class ContactMessage
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? AdditionalPhoneNumber { get; set; }
    public string? TelegramChatUrl { get; set; }
    public string? TelegramUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? LocationUrl { get; set; }
    public string? AboutClinic { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
