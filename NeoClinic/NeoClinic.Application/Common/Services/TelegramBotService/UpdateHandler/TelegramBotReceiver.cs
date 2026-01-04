using Microsoft.Extensions.DependencyInjection;
using NeoClinic.Application.Common.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

public class TelegramBotReceiver(ITelegramBotClient bot, IServiceProvider serviceProvider)
{
    public void InitialStartReceiving()
    {
        bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Console.WriteLine($"📩 Update received: {update.Message}");

        using var scope = serviceProvider.CreateScope();
        var commandHandler = scope.ServiceProvider.GetRequiredService<ICommandHandler>();
        var callbackHandler = scope.ServiceProvider.GetRequiredService<ICallbackHandler>();

        if (update.Message != null)
            await HandleMessage(update.Message, commandHandler);
        else if (update.CallbackQuery != null)
            await HandleCallback(update.CallbackQuery, callbackHandler);
    }

    private async Task HandleMessage(Message message, ICommandHandler commandHandler)
    {
        if (message.Text == null) return;

        if (message.Text == "/start")
            await commandHandler.HandleStartAsync(message);
        else if (message.Text == "➕ Add manager")
            await commandHandler.HandleAddManagerAsync(message);
        else if (message.Text == "❌ Remove manager")
            await commandHandler.HandleRemoveManagerAsync(message);
        else if (message.Text == "delete manager")
            await commandHandler.HandleDeleteManagerAsync(message);
        else if (Guid.TryParse(message.Text, out _))
            await commandHandler.HandleSetManagerAsync(message);
        else if (message.Text == "➕ Add admin")
            await commandHandler.HandleAddAdminAsync(message);
        else if (message.Text == "❌ Remove admin")
            await commandHandler.HandleRemoveAdminAsync(message);
        else if (message.Text == "delete admin")
            await commandHandler.HandleDeleteAdminAsync(message);
        else if (long.TryParse(message.Text, out _))
            await commandHandler.HandleSetAdminAsync(message);
        else if (message.Text == "🌐 Change language")
            await commandHandler.HandleLanguageChangeRequestAsync(message);
        else if (message.Text == "📝 Create profile")
            await commandHandler.HandleCreateProfileAsync(message);
        else if (message.Text == "📄 Get profile")
            await commandHandler.HandleGetProfileAsync(message);
        else
            await bot.SendMessage(
                chatId: message.Chat.Id,
                text: "❌ Unknown command. Please use the buttons or /start to begin."
            );
    }

    private async Task HandleCallback(CallbackQuery query, ICallbackHandler callbackHandler)
    {
        if (query.Data!.StartsWith("lang_"))
            await callbackHandler.HandleLanguageSelectionAsync(query);
        else if (query.Data!.StartsWith("approve_") || query.Data!.StartsWith("reject_"))
            await callbackHandler.HandleManagerActionAsync(query);
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
}
