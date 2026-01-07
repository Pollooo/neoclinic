using NeoClinic.Domain.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

public static class ReplyKeyboardHelper
{
    public static ReplyKeyboardMarkup GetDeveloperKeyboard()
    {
        return new ReplyKeyboardMarkup([["➕ Add manager"], ["❌ Remove manager"]])
        { ResizeKeyboard = true, OneTimeKeyboard = false };
    }

    public static ReplyKeyboardMarkup GetManagerKeyboard(Language lang)
    {
        return new ReplyKeyboardMarkup(
            lang switch
            {
                Language.Russian => [
                    ["➕ Добавить администратора"],
                    ["❌ Удалить администратора"],
                    ["📝 Создать профиль"],
                    ["📄 Получить профиль"]
                ],
                _ => [ // Default is Uzbek
                    ["➕ Admin qo'shish"],
                    ["❌ Adminni o'chirish"],
                    ["📝 Profil yaratish"],
                    ["📄 Profil olish"]
                ]
            })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }

    public static ReplyKeyboardMarkup GetAdminKeyboard(Language lang)
    {
        return new ReplyKeyboardMarkup(
            lang switch
            {
                Language.Russian => [
                    ["📝 Создать профиль"],
                    ["📄 Получить профиль"]
                ],
                _ => [ // Default is Uzbek
                    ["📝 Profil yaratish"],
                    ["📄 Profil olish"]
                ]
            })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
    }
}
