using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using NeoClinic.Domain.Enums;
using Telegram.Bot;

namespace NeoClinic.Application.UserCases.Appointments.CreateAppointment;

public class CreateAppointmentRequestHandler(
    IApplicationDbContext context,
    ITelegramBotClient bot)
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
            AppointmentDate = request.AppointmentDate.ToUniversalTime(),
        };
        await context.Appointments.AddAsync(appointment, cancellationToken);
        if (await context.SaveChangesAsync(cancellationToken) > 0)
            return await NotifyAboutAppointmentCreationAsync(request);

        return false;
    }

    private async Task<bool> NotifyAboutAppointmentCreationAsync(CreateAppointmentRequest appointmentRequest)
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
                        $"- Цена: {service.Price?.ToString("N2") ?? "-"} сум";
                }
                else
                {
                    serviceInfo =
                        $"🩺 Xizmat:\n" +
                        $"- Nomi: {service.NameUz}\n" +
                        $"- Tavsif: {service.DescriptionUz ?? "-"}\n" +
                        $"- Narx: {service.Price?.ToString("N2") ?? "-"} so'm";
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
                    $"🕒 Дата и время: {appointmentRequest.AppointmentDate:dd.MM.yyyy HH:mm}\n" +
                    $"{serviceInfo}";
            }
            else
            {
                message =
                    $"📅 Yangi qabul yaratildi\n" +
                    $"👤 Bemor: {appointmentRequest.PatientName}\n" +
                    $"📞 Telefon: {appointmentRequest.PhoneNumber}\n" +
                    $"✉ Email: {appointmentRequest.Email ?? "-"}\n" +
                    $"💬 Xabar: {appointmentRequest.Message ?? "-"}\n" +
                    $"🕒 Sana va vaqt: {appointmentRequest.AppointmentDate:dd.MM.yyyy HH:mm}\n" +
                    $"{serviceInfo}";
            }

            await bot.SendMessage(user.ChatId, message);
        }

        return true;
    }
}
