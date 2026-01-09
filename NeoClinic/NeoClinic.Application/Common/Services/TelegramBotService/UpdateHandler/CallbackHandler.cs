using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

public class CallbackHandler(IApplicationDbContext context, ITelegramBotClient bot) : ICallbackHandler
{
    public async Task HandleLanguageSelectionAsync(CallbackQuery query)
    {
        if (query.Data == null || query.Message == null) return;

        var user = await context.TelegramUsers.FirstOrDefaultAsync(u => u.ChatId == query.Message.Chat.Id);
        if (user is null) return;

        Language newLang = query.Data switch
        {
            "lang_russian" => Language.Russian,
            "lang_uzbek" => Language.Uzbek,
            _ => Language.Uzbek
        };

        user.Language = newLang;
        await context.SaveChangesAsync();

        string replyText = user.Language == Language.Russian
            ? "Язык успешно установлен ✅"
            : "Til muvaffaqiyatli o'rnatildi ✅";

        await bot.EditMessageText(
            chatId: user.ChatId,
            messageId: query.Message.Id,
            text: replyText
        );

        string waitingMessage = user.Language == Language.Russian
            ? "Пожалуйста, дождитесь подтверждения от менеджера ✅"
            : "Iltimos, menejer tomonidan tasdiqlanishini kuting ✅";

        await bot.SendMessage(
            chatId: user.ChatId,
            text: waitingMessage
        );

        var manager = await context.TelegramUsers.FirstOrDefaultAsync(m => m.IsManager);
        if (manager != null)
        {
            string managerMsg = manager.Language switch
            {
                Language.Russian => $"Новый пользователь [{user.FirstName}](tg://user?id={user.ChatId}) ожидает подтверждения. Выберите действие:",
                _ => $"Yangi foydalanuvchi [{user.FirstName}](tg://user?id={user.ChatId}) tasdiqlanishini kutmoqda. Harakatni tanlang:",
            };

            await bot.SendMessage(
                chatId: manager.ChatId,
                text: managerMsg,
                replyMarkup: InlineKeyboardHelper.GetManagerConfirmationKeyboard(user.Id),
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }
    }

    public async Task HandleManagerActionAsync(CallbackQuery query)
    {
        if (query.Data == null || query.Message == null) return;

        var parts = query.Data.Split('_');
        if (parts.Length != 2) return;

        string action = parts[0];
        if (!Guid.TryParse(parts[1], out var userId)) return;

        var manager = await context.TelegramUsers
            .FirstOrDefaultAsync(u => u.ChatId == query.Message.Chat.Id && u.IsManager);

        if (manager == null) return;

        var managerLanguage = manager.Language;

        var user = await context.TelegramUsers.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return;
        var userLanguage = user.Language;
        string responseText = string.Empty;
        string actionText = string.Empty;

        if (action == "approve")
        {
            user.IsVarified = true;
            user.IsAdmin = true;

            responseText = userLanguage switch
            {
                Language.Russian => "Вы были успешно подтверждены менеджером ✅",
                _ => "Siz menejer tomonidan muvaffaqiyatli tasdiqlandingiz ✅",
            };

            actionText = managerLanguage switch
            {
                Language.Russian => "Подтверждено ✅",
                _ => "Tasdiqlandi ✅",
            };

            await bot.SendMessage(
                chatId: user.ChatId,
                text: userLanguage switch
                {
                    Language.Russian => "🎉 Теперь вы администратор. Используйте кнопки ниже для управления профилями.",
                    _ => "🎉 Endi siz adminsiz. Quyidagi tugmalar orqali profillarni boshqarishingiz mumkin.",
                },
                replyMarkup: ReplyKeyboardHelper.GetAdminKeyboard(user.Language)
            );
        }
        else
        {
            user.IsVarified = false;

            responseText = userLanguage switch
            {
                Language.Russian => "К сожалению, менеджер отклонил вашу заявку ❌",
                _ => "Afsus, menejer sizning arizangizni rad etdi ❌",
            };

            actionText = userLanguage switch
            {
                Language.Russian => "Отклонено ❌",
                _ => "Rad etildi ❌"
            };
        }

        context.TelegramUsers.Update(user);
        await context.SaveChangesAsync();

        await bot.SendMessage(
            chatId: user.ChatId,
            text: responseText
        );

        string managerMessageText = managerLanguage switch
        {
            Language.Russian => $"Действие выполнено: {actionText}",
            _ => $"Harakat bajarildi: {actionText}",
        };

        await bot.EditMessageText(
            chatId: query.Message.Chat.Id,
            messageId: query.Message.Id,
            text: managerMessageText
        );
    }
}
