using Telegram.Bot.Types.ReplyMarkups;

namespace NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

public static class InlineKeyboardHelper
{
    public static InlineKeyboardMarkup GetLanguageSelectionKeyboard()
    {
        return new InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton.WithCallbackData("🇷🇺 Русский", "lang_russian"),
                InlineKeyboardButton.WithCallbackData("🇺🇿 O`zbek", "lang_uzbek")
            ]
        ]);
    }
    public static InlineKeyboardMarkup GetManagerConfirmationKeyboard(Guid userId)
    {
        return new InlineKeyboardMarkup(
        [
            [
                InlineKeyboardButton.WithCallbackData("✅ Approve", $"approve_{userId}"),
                InlineKeyboardButton.WithCallbackData("❌ Reject", $"reject_{userId}")
            ]
        ]);
    }
}
