namespace NeoClinic.Domain.Entities;

public class Admin
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public Guid TelegramUserId { get; set; }
    public TelegramUser TelegramUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
