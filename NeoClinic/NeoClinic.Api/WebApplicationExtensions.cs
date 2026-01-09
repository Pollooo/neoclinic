using NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

namespace NeoClinic.Api;

public static class WebApplicationExtensions
{
    extension(WebApplication app)
    {
        public WebApplication StartTelegramBot()
        {
            try
            {
                var receiver = app.Services.GetRequiredService<TelegramBotReceiver>();
                receiver.InitialStartReceiving();
                Console.WriteLine("✅ Telegram bot started successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Warning: Telegram bot could not start: {ex.Message}");
                Console.WriteLine("The API will continue to run, but Telegram features will be unavailable.");
            }

            return app;
        }
    }
}
