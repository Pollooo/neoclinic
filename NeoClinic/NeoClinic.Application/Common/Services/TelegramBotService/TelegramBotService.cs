using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Application.UserCases.Appointments.CreateAppointment;

namespace NeoClinic.Application.Common.Services.TelegramBotService;

public class TelegramBotService : ITelegramBotService
{
    public Task<bool> NotifyAboutAppointmentCreationAsync(CreateAppointmentRequest appointmentRequest)
    {
        throw new NotImplementedException();
    }

    public Task NotifyAboutErrorAsync(string error, string? handler)
    {

        throw new NotImplementedException();
    }

    public Task NotifyEveryoneAboutLogin()
    {

        throw new NotImplementedException();
    }
}
