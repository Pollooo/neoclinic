using MediatR;
using NeoClinic.Application.UserCases.ErrorLogs.Get;

namespace NeoClinic.Api.Endpoints;

public static class ErrorLogEndpoints
{
    private const string GroupName = "api/error-logs";

    public static void MapErrorLogEndpoints(this WebApplication app)
    {
        app.MapGet($"{GroupName}/get", GetErrorLogsAsync)
           .RequireAuthorization("AdminPolicy");
    }

    private static async Task<IResult> GetErrorLogsAsync(
        int page = 1,
        int pageSize = 10,
        ISender sender = default!)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var result = await sender.Send(new GetErrorLogsRequest
        {
            Page = page,
            PageSize = pageSize
        });

        return Results.Ok(result);
    }
}
