using Telegram.Bot.Types.ReplyMarkups;

namespace NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

public static class ReplyKeyboardHelper
{
    public static ReplyKeyboardMarkup GetDeveloperKeyboard()
    {
        return new ReplyKeyboardMarkup(
        [
            ["➕ Add manager"],
            ["❌ Remove manager"]
        ])
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }

    public static ReplyKeyboardMarkup GetManagerKeyboard()
    {
        return new ReplyKeyboardMarkup(
        [
            ["➕ Add admin"],
            ["❌ Remove admin"]
        ])
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }

    public static ReplyKeyboardMarkup GetUserKeyboard()
    {
        return new ReplyKeyboardMarkup(
            [
                ["🌐 Change language"],
            ])
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }

    public static ReplyKeyboardMarkup GetAdminKeyboard()
    {
        return new ReplyKeyboardMarkup(
            [
                ["📝 Create profile"],
                ["📄 Get profile"],
            ])
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }
}
