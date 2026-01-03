namespace NeoClinic.Domain.Entities;

public class TelegramUser
{
    public Guid Id { get; set; }
    public long ChatId { get; set; }
    public string? Username { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? LanguageCode { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsVarified { get; set; } = true;
    public bool IsBotBlocked { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
    public Admin? Admin { get; set; }
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}
