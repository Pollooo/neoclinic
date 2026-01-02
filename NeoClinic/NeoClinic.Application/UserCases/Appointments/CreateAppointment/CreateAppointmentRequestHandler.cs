using MediatR;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using System.Text.Json;

namespace NeoClinic.Application.UserCases.Appointments.CreateAppointment;

public class CreateAppointmentRequestHandler(
    IApplicationDbContext context,
    ITelegramBotService botService)
    : IRequestHandler<CreateAppointmentRequest, bool>
{
    public async Task<bool> Handle(CreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var appointment = new Appointment
        {
            PatientName = request.PatientName,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Message = request.Message,
            ServiceId = request.ServiceId,
            AppointmentDate = request.AppointmentDate,
        };
        await context.Appointments.AddAsync(appointment, cancellationToken);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return await botService.NotifyAboutAppointmentCreationAsync(request);
        else
            await botService.NotifyAboutErrorAsync(JsonSerializer.Serialize(request), "CreateAppointmentRequestHandler");
        return false;
    }
}
