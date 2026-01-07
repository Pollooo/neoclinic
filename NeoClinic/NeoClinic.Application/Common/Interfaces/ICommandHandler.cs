using Telegram.Bot.Types;

namespace NeoClinic.Application.Common.Interfaces;

public interface ICommandHandler
{
    Task HandleStartAsync(Message message);
    Task HandleAddManagerAsync(Message message);
    Task HandleRemoveManagerAsync(Message message);
    Task HandleDeleteManagerAsync(Message message);
    Task HandleSetManagerAsync(Message message);
    Task HandleAddAdminAsync(Message message);
    Task HandleSetAdminAsync(Message message);
    Task HandleRemoveAdminAsync(Message message);
    Task HandleDeleteAdminAsync(Message message);
    Task HandleCreateProfileAsync(Message message);
    Task HandleGetProfileAsync(Message message);
}
