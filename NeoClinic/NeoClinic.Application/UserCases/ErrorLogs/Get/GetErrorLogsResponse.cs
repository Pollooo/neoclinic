namespace NeoClinic.Application.UserCases.ErrorLogs.Get;

public class GetErrorLogsResponse
{
    public List<ErrorLogItem> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class ErrorLogItem
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public string? Path { get; set; }
    public string? Method { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
}
