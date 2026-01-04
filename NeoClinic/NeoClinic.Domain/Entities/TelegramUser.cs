using NeoClinic.Domain.Enums;

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
    public bool IsDeveloper { get; set; } = false;
    public bool IsManager { get; set; } = false;
    public bool IsVarified { get; set; } = false;
    public bool IsBotBlocked { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
    public Language Language { get; set; }
    public Admin? Admin { get; set; }
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}
