namespace NeoClinic.Domain.Entities;

public class ErrorLog
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public int StatusCode { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? UserId { get; set; }
    public string? RequestBody { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
