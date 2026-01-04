using NeoClinic.Application.UserCases.Appointments.CreateAppointment;

namespace NeoClinic.Application.Common.Interfaces;

public interface ITelegramBotService
{
    Task<bool> NotifyAboutAppointmentCreationAsync(CreateAppointmentRequest appointmentRequest);
    Task NotifyEveryoneAboutLogin(string userName);
    Task NotifyAboutErrorAsync(string error, string? handler);
}
