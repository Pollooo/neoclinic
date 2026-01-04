using Telegram.Bot.Types;

namespace NeoClinic.Application.Common.Interfaces;

public interface ICallbackHandler
{
    Task HandleLanguageSelectionAsync(CallbackQuery query);
    Task HandleManagerActionAsync(CallbackQuery query);
}
