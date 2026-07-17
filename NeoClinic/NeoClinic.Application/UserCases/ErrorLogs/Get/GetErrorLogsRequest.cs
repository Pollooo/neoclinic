using MediatR;

namespace NeoClinic.Application.UserCases.ErrorLogs.Get;

public class GetErrorLogsRequest : IRequest<GetErrorLogsResponse>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
