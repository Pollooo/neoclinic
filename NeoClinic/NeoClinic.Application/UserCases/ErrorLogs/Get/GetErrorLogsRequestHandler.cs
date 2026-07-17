using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.ErrorLogs.Get;

public class GetErrorLogsRequestHandler(IApplicationDbContext context)
    : IRequestHandler<GetErrorLogsRequest, GetErrorLogsResponse>
{
    public async Task<GetErrorLogsResponse> Handle(GetErrorLogsRequest request, CancellationToken cancellationToken)
    {
        var query = context.ErrorLogs.OrderByDescending(e => e.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new ErrorLogItem
            {
                Id = e.Id,
                Message = e.Message,
                StackTrace = e.StackTrace,
                Source = e.Source,
                Path = e.Path,
                Method = e.Method,
                StatusCode = e.StatusCode,
                Timestamp = e.Timestamp
            })
            .ToListAsync(cancellationToken);

        return new GetErrorLogsResponse
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
