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

        string replyText = string.Empty;

        switch (query.Data)
        {
            case "lang_russian":
                if (user.Language == Language.Russian)
                    replyText = "Язык уже установлен на Русский ✅";
                else
                {
                    user.Language = Language.Russian;
                    replyText = "Язык успешно изменён на Русский ✅";
                }
                break;

            case "lang_uzbek":
                if (user.Language == Language.Uzbek)
                    replyText = "Til allaqachon O‘zbek tiliga o'rnatilgan✅";
                else
                {
                    user.Language = Language.Uzbek;
                    replyText = "Til muvaffaqiyatli O‘zbek tiliga o‘zgartirildi ✅";
                }
                break;

            default:
                user.Language = Language.Uzbek;
                replyText = "O‘zbek tili avtomatik ravishda tanlandi, ehtimol ichki xatolik tufayli ✅";
                break;
        }

        await context.SaveChangesAsync();

        await bot.EditMessageText(
            chatId: user.ChatId,
            messageId: query.Message!.MessageId,
            text: replyText
        );

        string waitingMessage = user.Language switch
        {
            Language.Russian => "Пожалуйста, дождитесь подтверждения от менеджера, чтобы быть принятым как сотрудник или отклонён ✅",
            Language.Uzbek => "Iltimos, menejer tomonidan tasdiqlanishini kuting, xodim sifatida qabul qilinish yoki rad etilish ✅",
            _ => "Iltimos, menejer tomonidan tasdiqlanishini kuting ✅" // fallback
        };

        await bot.SendMessage(
            chatId: user.ChatId,
            text: waitingMessage,
            replyMarkup: ReplyKeyboardHelper.GetUserKeyboard()
        );

        var manager = await context.TelegramUsers.FirstOrDefaultAsync(m => m.IsManager);
        if (manager is null) return;

        string managerMessage = user.Language switch
        {
            Language.Russian => $"Пользователь [{user.FirstName}](tg://user?id={user.ChatId}) ожидает подтверждения. Выберите действие:",
            Language.Uzbek => $"Foydalanuvchi [{user.FirstName}](tg://user?id={user.ChatId}) tasdiqlanishini kutmoqda. Harakatni tanlang:",
            _ => $"Foydalanuvchi [{user.FirstName}](tg://user?id={user.ChatId}) tasdiqlanishini kutmoqda. Harakatni tanlang:" // fallback
        };

        await bot.SendMessage(
            chatId: manager.ChatId,
            text: managerMessage,
            replyMarkup: InlineKeyboardHelper.GetManagerConfirmationKeyboard(user.Id),
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );
    }

    public async Task HandleManagerActionAsync(CallbackQuery query)
    {
        if (query.Data == null || query.Message == null) return;

        var parts = query.Data.Split('_');
        if (parts.Length != 2) return;

        string action = parts[0];
        if (!Guid.TryParse(parts[1], out var userId)) return;

        var user = await context.TelegramUsers.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return;

        string responseText = string.Empty;
        string actionText = string.Empty;

        var language = user.Language != 0 ? user.Language : Language.Uzbek;

        if (action == "approve")
        {
            user.IsVarified = true;

            responseText = language switch
            {
                Language.Russian => "Вы были успешно подтверждены менеджером ✅",
                Language.Uzbek => "Siz menejer tomonidan muvaffaqiyatli tasdiqlandingiz ✅",
                _ => "Siz menejer tomonidan tasdiqlandingiz ✅"
            };

            actionText = language switch
            {
                Language.Russian => "Подтверждено ✅",
                Language.Uzbek => "Tasdiqlandi ✅",
                _ => "Approved ✅"
            };

            await bot.SendMessage(
                chatId: user.ChatId,
                text: language switch
                {
                    Language.Russian => "🎉 Теперь вы администратор. Используйте кнопки ниже для управления профилями.",
                    Language.Uzbek => "🎉 Endi siz adminsiz. Quyidagi tugmalar orqali profillarni boshqarishingiz mumkin.",
                    _ => "🎉 You are now an admin. Use the buttons below to manage profiles."
                },
                replyMarkup: ReplyKeyboardHelper.GetAdminKeyboard()
            );
        }
        else
        {
            user.IsVarified = false;

            responseText = language switch
            {
                Language.Russian => "К сожалению, менеджер отклонил вашу заявку ❌",
                Language.Uzbek => "Afsus, menejer sizning arizangizni rad etdi ❌",
                _ => "Afsus, menejer sizning arizangizni rad etdi ❌"
            };

            actionText = language switch
            {
                Language.Russian => "Отклонено ❌",
                Language.Uzbek => "Rad etildi ❌",
                _ => "Rejected ❌"
            };
        }

        context.TelegramUsers.Update(user);
        await context.SaveChangesAsync();

        await bot.SendMessage(
            chatId: user.ChatId,
            text: responseText
        );

        string managerMessageText = language switch
        {
            Language.Russian => $"Действие выполнено: {actionText}",
            Language.Uzbek => $"Harakat bajarildi: {actionText}",
            _ => $"Action completed: {actionText}"
        };

        await bot.EditMessageText(
            chatId: query.Message.Chat.Id,
            messageId: query.Message.MessageId,
            text: managerMessageText
        );
    }
}
