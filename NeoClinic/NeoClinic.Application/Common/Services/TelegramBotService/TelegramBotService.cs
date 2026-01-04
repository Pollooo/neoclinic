using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Application.UserCases.Appointments.CreateAppointment;
using NeoClinic.Domain.Enums;
using Telegram.Bot;

namespace NeoClinic.Application.Common.Services.TelegramBotService;

public class TelegramBotService(
    ITelegramBotClient bot,
    IConfiguration configuration,
    IApplicationDbContext context) : ITelegramBotService
{
    private readonly ITelegramBotClient Bot = bot;
    private readonly string ChatId = configuration["Telegram:ChatId"]!;

    public async Task<bool> NotifyAboutAppointmentCreationAsync(CreateAppointmentRequest appointmentRequest)
    {
        var service = await context.Services
            .FirstOrDefaultAsync(s => s.Id == appointmentRequest.ServiceId);

        var approvedUsers = await context.TelegramUsers
            .Where(u => u.IsVarified)
            .ToListAsync();

        foreach (var user in approvedUsers)
        {
            string message;

            string serviceInfo;
            if (service != null)
            {
                if (user.Language == Language.Russian)
                {
                    serviceInfo =
                        $"🩺 Услуга:\n" +
                        $"- Название: {service.NameRu}\n" +
                        $"- Описание: {service.DescriptionRu ?? "-"}\n" +
                        $"- Цена: {service.Price?.ToString("C") ?? "-"}";
                }
                else
                {
                    serviceInfo =
                        $"🩺 Xizmat:\n" +
                        $"- Nomi: {service.NameUz}\n" +
                        $"- Tavsif: {service.DescriptionUz ?? "-"}\n" +
                        $"- Narx: {service.Price?.ToString("C") ?? "-"}";
                }
            }
            else
            {
                serviceInfo = user.Language == Language.Russian ? "🩺 Услуга: -" : "🩺 Xizmat: -";
            }

            if (user.Language == Language.Russian)
            {
                message =
                    $"📅 Создана новая запись на прием\n" +
                    $"👤 Пациент: {appointmentRequest.PatientName}\n" +
                    $"📞 Телефон: {appointmentRequest.PhoneNumber}\n" +
                    $"✉ Email: {appointmentRequest.Email ?? "-"}\n" +
                    $"💬 Сообщение: {appointmentRequest.Message ?? "-"}\n" +
                    $"{serviceInfo}\n" +
                    $"🕒 Дата и время: {appointmentRequest.AppointmentDate:dd.MM.yyyy HH:mm}";
            }
            else
            {
                message =
                    $"📅 Yangi qabul yaratildi\n" +
                    $"👤 Bemor: {appointmentRequest.PatientName}\n" +
                    $"📞 Telefon: {appointmentRequest.PhoneNumber}\n" +
                    $"✉ Email: {appointmentRequest.Email ?? "-"}\n" +
                    $"💬 Xabar: {appointmentRequest.Message ?? "-"}\n" +
                    $"{serviceInfo}\n" +
                    $"🕒 Sana va vaqt: {appointmentRequest.AppointmentDate:dd.MM.yyyy HH:mm}";
            }

            await Bot.SendMessage(user.ChatId, message);
        }

        return true;
    }

    public async Task NotifyAboutErrorAsync(string error, string? handler)
    {
        var message =
            $"❌ ERROR\n" +
            $"Handler: {handler ?? "Unknown"}\n" +
            $"Message: {error}";

        await Bot.SendMessage(ChatId, message);
    }

    public async Task NotifyEveryoneAboutLogin(string userName)
    {
        var approvedUsers = await context.TelegramUsers
            .Where(u => u.IsVarified)
            .ToListAsync();

        foreach (var user in approvedUsers)
        {
            string message;

            if (user.Language == Language.Russian)
            {
                message = $"🔐 {userName} вошел(а) в систему";
            }
            else
            {
                message = $"🔐 {userName} tizimga kirdi";
            }

            await Bot.SendMessage(user.ChatId, message);
        }
    }
}
