using System.Diagnostics;
using System.Text;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Api.Middleware;

public class ExceptionLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApplicationDbContext dbContext)
    {
        var stopwatch = Stopwatch.StartNew();
        string? requestBody = null;

        if (context.Request.ContentLength > 0 && context.Request.ContentLength < 50000)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        try
        {
            await _next(context);

            if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
            {
                var error = new ErrorLog
                {
                    Id = Guid.NewGuid(),
                    Message = $"HTTP {context.Response.StatusCode}: {GetDefaultMessage(context.Response.StatusCode)}",
                    StatusCode = context.Response.StatusCode,
                    Path = context.Request.Path,
                    Method = context.Request.Method,
                    Source = "ClientError",
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers.UserAgent.FirstOrDefault(),
                    UserId = context.User.Identity?.IsAuthenticated == true
                        ? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                        : null,
                    RequestBody = requestBody,
                    Timestamp = DateTime.UtcNow
                };
                dbContext.ErrorLogs.Add(error);
                await TrySaveChangesAsync(dbContext);
            }
        }
        catch (Exception ex)
        {
            var error = new ErrorLog
            {
                Id = Guid.NewGuid(),
                Message = $"{ex.GetType().Name}: {ex.Message}",
                StackTrace = ex.ToString(),
                Source = ex.Source,
                StatusCode = 500,
                Path = context.Request.Path,
                Method = context.Request.Method,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers.UserAgent.FirstOrDefault(),
                UserId = context.User.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                    : null,
                RequestBody = requestBody,
                Timestamp = DateTime.UtcNow
            };
            dbContext.ErrorLogs.Add(error);
            await TrySaveChangesAsync(dbContext);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(
                """{"message":"An unexpected error occurred. Please try again later.","statusCode":500}""");
        }
        finally
        {
            stopwatch.Stop();
            if (stopwatch.ElapsedMilliseconds > 3000)
            {
                Console.WriteLine($"[SLOW] {context.Request.Method} {context.Request.Path} took {stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }

    private static async Task TrySaveChangesAsync(IApplicationDbContext dbContext)
    {
        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ErrorLog] Failed to save error log: {ex.Message}");
        }
    }

    private static string GetDefaultMessage(int statusCode) => statusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        409 => "Conflict",
        422 => "Unprocessable Entity",
        429 => "Too Many Requests",
        _ => "Client Error"
    };
}
