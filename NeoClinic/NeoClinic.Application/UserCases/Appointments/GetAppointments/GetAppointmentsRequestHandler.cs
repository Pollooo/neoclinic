using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Application.UserCases.Services.Get;

namespace NeoClinic.Application.UserCases.Appointments.GetAppointments;

public class GetAppointmentsRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetAppointmentsRequest, List<GetAppointmentsResponse>?>
{
    public async Task<List<GetAppointmentsResponse>?> Handle(GetAppointmentsRequest request, CancellationToken cancellationToken)
    {
        var todayUtc = DateTime.UtcNow.Date;

        var startDate = request.StartDate?.ToDateTime(TimeOnly.MinValue).ToUniversalTime()
                        ?? todayUtc;

        var endDate = request.EndDate?.ToDateTime(TimeOnly.MaxValue).ToUniversalTime()
                      ?? todayUtc.AddDays(30);

        return await context.Appointments
            .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
            .Select(a => new GetAppointmentsResponse(
                a.Id,
                a.PatientName,
                a.PhoneNumber,
                a.Email,
                a.Message,
                a.AppointmentDate,
                new GetServicesResponse(
                    a.Service.Id,
                    a.Service.NameUz,
                    a.Service.DescriptionUz,
                    a.Service.NameRu,
                    a.Service.DescriptionRu,
                    a.Service.Price),
                a.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
