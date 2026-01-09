using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using NeoClinic.Domain.Enums;
using System.Security.Cryptography;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;

public class CommandHandler(
    IApplicationDbContext context,
    ITelegramBotClient bot,
    IConfiguration configuration) : ICommandHandler
{
    public async Task HandleStartAsync(Message message)
    {
        var firstName = message.From?.FirstName;
        var telegramId = message.From?.Id;
        var chatId = message.Chat.Id;
        var userExists = await context.TelegramUsers.AnyAsync(t => t.ChatId == chatId);

        if (!userExists)
        {
            await bot.SendMessage(
                chatId,
                text: $"👋 [{firstName}](tg://user?id={telegramId})!",
                parseMode: ParseMode.Markdown
            );

            await bot.SendMessage(
                chatId,
                text: "Пожалуйста, выберите язык! \nIltimos, tilni tanlang!",
                replyMarkup: InlineKeyboardHelper.GetLanguageSelectionKeyboard()
            );
        }
        else
        {
            await bot.SendMessage(
                chatId,
                text: $"👋 [{firstName}](tg://user?id={telegramId})!",
                parseMode: ParseMode.Markdown
            );
        }

        var developerChatId = long.Parse(configuration["Telegram:ChatId"]!);

        var user = new TelegramUser
        {
            ChatId = chatId,
            Username = message.From?.Username,
            FirstName = message.From?.FirstName ?? string.Empty,
            LastName = message.From?.LastName,
            LanguageCode = message.From?.LanguageCode,
            IsAdmin = chatId == developerChatId,
            IsVarified = chatId == developerChatId,
            SubscribedAt = DateTime.UtcNow
        };

        if (chatId == developerChatId)
        {
            await bot.SendMessage(
                chatId,
                text: "👨\u200D💻 Welcome, Developer! \nChoose an action:",
                replyMarkup: ReplyKeyboardHelper.GetDeveloperKeyboard(),
                parseMode: ParseMode.Markdown
            );
            user.IsVarified = true;
            user.IsDeveloper = true;
            user.IsManager = true;
        }

        if (userExists) return;

        await context.TelegramUsers.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task HandleAddManagerAsync(Message message)
    {
        var existingManager = await context.TelegramUsers.AnyAsync(u => u.IsManager);
        if (existingManager)
        {
            await bot.SendMessage(
                chatId: message.Chat.Id,
                 text: "❌ Manager already exists.\n" +
                       "If you want to change the manager, first remove the current one and then set a new manager."
            );
            return;
        }

        var developer = await context.TelegramUsers.FirstOrDefaultAsync(d => d.IsDeveloper);
        if (developer is null || message.Chat.Id != developer.ChatId)
        {
            await bot.SendMessage(
                chatId: message.Chat.Id,
                text: "❌ Developer not found in database."
            );
            return;
        }

        var users = await context.TelegramUsers
        .Where(u => u.IsVarified)
        .ToListAsync();

        if (users.Count == 0)
        {
            await bot.SendMessage(
                chatId: message.Chat.Id,
                text: "No verified users found to set as manager."
            );
            return;
        }

        string usersList = string.Join("\n", users.Select(u =>
            $"[{u.FirstName}](tg://user?id={u.ChatId}) | `{u.Id}`"
        ));

        await bot.SendMessage(
            chatId: message.Chat.Id,
            text: $"📋 Verified users:\n{usersList}",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );

        await bot.SendMessage(
            chatId: message.Chat.Id,
            text: "📌 Please copy and paste the **USER ID (GUID)** of the user you want to set as manager.\n" +
                  "⚠️ Do NOT send Chat ID.",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );
    }

    public async Task HandleRemoveManagerAsync(Message message)
    {
        var developer = await context.TelegramUsers.FirstOrDefaultAsync(d => d.IsDeveloper);
        if (developer is null)
        {
            await bot.SendMessage(
                chatId: message.Chat.Id,
                text: "❌ Developer not found in database."
            );
            return;
        }

        var manager = await context.TelegramUsers.FirstOrDefaultAsync(u => u.IsManager);
        if (manager is null)
        {
            await bot.SendMessage(
                chatId: developer.ChatId,
                text: "❌ There is no manager to remove."
            );
            return;
        }

        await bot.SendMessage(
            chatId: developer.ChatId,
            text: $"⚠️ You are about to remove the manager: {manager.FirstName}.\n" +
                  $"Type `delete manager` to confirm.",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );
    }

    public async Task HandleDeleteManagerAsync(Message message)
    {
        var developer = await context.TelegramUsers.FirstOrDefaultAsync(d => d.IsDeveloper);
        if (developer is null)
        {
            await bot.SendMessage(
                chatId: message.Chat.Id,
                text: "❌ Developer not found in database."
            );
            return;
        }

        var manager = await context.TelegramUsers.FirstOrDefaultAsync(u => u.IsManager);
        if (manager is null)
        {
            await bot.SendMessage(
                chatId: developer.ChatId,
                text: "❌ There is no manager to remove."
            );
            return;
        }

        // Unassign the manager
        manager.IsManager = false;
        context.TelegramUsers.Update(manager);
        await context.SaveChangesAsync();

        // Notify the developer
        await bot.SendMessage(
            chatId: developer.ChatId,
            text: $"✅ Manager {manager.FirstName} has been removed successfully.",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );

        // Notify the former manager
        await bot.SendMessage(
            chatId: manager.ChatId,
            text: manager.Language switch
            {
                Language.Russian => "⚠️ Вы больше не менеджер. Ваши возможности управления администраторами отключены.",
                Language.Uzbek => "⚠️ Siz endi menejer emassiz. Adminlarni boshqarish imkoniyatingiz o‘chirilgan.",
                _ => "⚠️ You are no longer a manager. Your admin controls have been disabled."
            }
        );
    }

    public async Task HandleSetManagerAsync(Message message)
    {
        var developer = await context.TelegramUsers.FirstOrDefaultAsync(d => d.IsDeveloper);
        if (developer is null)
        {
            await bot.SendMessage(
                chatId: message.Chat.Id,
                text: "❌ Developer not found in database."
            );
            return;
        }

        // Only developer can do this
        if (message.Chat.Id != developer.ChatId)
            return;

        // Parse RECORD ID (GUID)
        if (!Guid.TryParse(message.Text, out var userId))
        {
            await bot.SendMessage(
                chatId: developer.ChatId,
                text: "❌ Invalid ID format.\nPlease send a valid USER ID (GUID)."
            );
            return;
        }

        var user = await context.TelegramUsers
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsVarified);

        if (user is null)
        {
            await bot.SendMessage(
                chatId: developer.ChatId,
                text: "❌ User not found or not verified.\nMake sure the USER ID is correct."
            );
            return;
        }

        var existingManager = await context.TelegramUsers.FirstOrDefaultAsync(u => u.IsManager);
        if (existingManager is not null)
        {
            await bot.SendMessage(
                chatId: developer.ChatId,
                text: $"❌ Manager {existingManager.FirstName} already exists.\nRemove them first to set a new manager."
            );
            return;
        }

        // Set manager
        user.IsManager = true;
        user.IsAdmin = true;
        context.TelegramUsers.Update(user);
        await context.SaveChangesAsync();

        // Notify developer
        await bot.SendMessage(
            chatId: developer.ChatId,
            text: $"✅ User {user.FirstName} has been successfully set as the new manager.\nID: `{user.Id}`",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );

        // Notify manager (language-based)
        string managerText = user.Language switch
        {
            Language.Russian =>
                "🎉 Вы назначены менеджером.\nВы можете управлять администраторами с помощью кнопок ниже.",

            Language.Uzbek =>
                "🎉 Siz menejer etib tayinlandingiz.\nQuyidagi tugmalar orqali adminlarni boshqarishingiz mumkin.",

            _ =>
                "🎉 You are now a manager."
        };

        await bot.SendMessage(
            chatId: user.ChatId,
            text: managerText,
            replyMarkup: ReplyKeyboardHelper.GetManagerKeyboard(user.Language)
        );
    }

    public async Task HandleAddAdminAsync(Message message)
    {
        var manager = await context.TelegramUsers
            .FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id && u.IsManager);
        if (manager is null) return;

        var language = manager.Language; // now it's Language enum

        var users = await context.TelegramUsers
            .Where(u => u.IsVarified && !u.IsAdmin)
            .ToListAsync();

        if (users.Count == 0)
        {
            string text = language switch
            {
                Language.Russian => "❌ Нет пользователей для назначения администратором.",
                _ => "❌ Admin qilib belgilash uchun foydalanuvchi mavjud emas." // default Uzbek
            };

            await bot.SendMessage(
                chatId: manager.ChatId,
                text: text
            );
            return;
        }

        string list = string.Join("\n", users.Select(u =>
            $"[{u.FirstName}](tg://user?id={u.ChatId}) | {u.Username} | `{u.ChatId}`"
        ));

        string verifiedText = language switch
        {
            Language.Russian => $"📋 Проверенные пользователи:\n{list}",
            _ => $"📋 Tasdiqlangan foydalanuvchilar:\n{list}" // default Uzbek
        };

        string promptText = language switch
        {
            Language.Russian => "📌 Скопируйте и отправьте Chat ID для назначения админа.",
            _ => "📌 Chat ID ni nusxalab yuboring, admin belgilash uchun." // default Uzbek
        };

        await bot.SendMessage(
            chatId: manager.ChatId,
            text: verifiedText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );

        await bot.SendMessage(
            chatId: manager.ChatId,
            text: promptText
        );
    }

    public async Task HandleSetAdminAsync(Message message)
    {
        var manager = await context.TelegramUsers
            .FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id && u.IsManager);

        if (manager is null)
            return;

        if (!long.TryParse(message.Text, out var chatId))
            return;

        var user = await context.TelegramUsers.FirstOrDefaultAsync(u => u.ChatId == chatId);
        if (user is null)
        {
            await bot.SendMessage(
                chatId: manager.ChatId,
                text: "❌ User not found."
            );
            return;
        }

        user.IsAdmin = true;
        await context.SaveChangesAsync();

        await bot.SendMessage(
            chatId: manager.ChatId,
            text: $"✅ {user.FirstName} is now an admin."
        );

        await bot.SendMessage(
            chatId: user.ChatId,
            text: "🎉 You have been promoted to admin."
        );
    }

    public async Task HandleRemoveAdminAsync(Message message)
    {
        var manager = await context.TelegramUsers
            .FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id && u.IsManager);

        if (manager is null)
            return;

        var admins = await context.TelegramUsers
            .Where(u => u.IsAdmin)
            .ToListAsync();

        if (admins.Count == 0)
        {
            await bot.SendMessage(
                chatId: manager.ChatId,
                text: "❌ No admins to remove."
            );
            return;
        }

        string list = string.Join("\n", admins.Select(u =>
            $"[{u.FirstName}](tg://user?id={u.ChatId}) | `{u.ChatId}`"
        ));

        await bot.SendMessage(
            chatId: manager.ChatId,
            text: $"⚠️ Admins:\n{list}\n\nType `delete admin` or admin name to confirm removal.",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );
    }

    public async Task HandleDeleteAdminAsync(Message message)
    {
        var manager = await context.TelegramUsers
            .FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id && u.IsManager);

        if (manager is null)
            return;

        var admin = await context.TelegramUsers.FirstOrDefaultAsync(u => u.IsAdmin);
        if (admin is null)
        {
            await bot.SendMessage(
                chatId: manager.ChatId,
                text: manager.Language switch
                {
                    Language.Russian => "❌ Админ не найден.",
                    Language.Uzbek => "❌ Admin topilmadi.",
                    _ => "❌ No admin found."
                }
            );
            return;
        }

        admin.IsAdmin = false;
        context.TelegramUsers.Update(admin);
        var adminAccount = await context.Admins.FirstOrDefaultAsync(a => a.TelegramUserId == admin.Id);
        if (adminAccount is not null)
        {
            context.Admins.Remove(adminAccount);
        }
        await context.SaveChangesAsync();

        await bot.SendMessage(
            chatId: manager.ChatId,
            text: manager.Language switch
            {
                Language.Russian => $"✅ Админ {admin.FirstName} был удалён.",
                Language.Uzbek => $"✅ Admin {admin.FirstName} olib tashlandi.",
                _ => $"✅ Admin {admin.FirstName} has been removed."
            }
        );

        await bot.SendMessage(
            chatId: admin.ChatId,
            text: admin.Language switch
            {
                Language.Russian => "⚠️ Ваша роль админа была удалена.",
                Language.Uzbek => "⚠️ Sizning admin rolingiz olib tashlandi.",
                _ => "⚠️ Your admin role has been removed."
            }
        );
    }

    public async Task HandleCreateProfileAsync(Message message)
    {
        var user = await context.TelegramUsers.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
        if (user is null) return;

        var existingAdmin = await context.Admins.FirstOrDefaultAsync(a => a.TelegramUserId == user.Id);
        if (existingAdmin is not null)
        {
            await bot.SendMessage(
                chatId: user.ChatId,
                text: user.Language switch
                {
                    Language.Russian => $"📝 У вас уже есть профиль!\n\nИмя пользователя: `{existingAdmin.Username}`\nПароль: `******`",
                    Language.Uzbek => $"📝 Sizda allaqachon profil mavjud!\n\nFoydalanuvchi nomi: `{existingAdmin.Username}`\nParol: `******`",
                    _ => $"📝 Sizda allaqachon profil mavjud!\n\nFoydalanuvchi nomi: `{existingAdmin.Username}`\nParol: `******`"
                },
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );

            return;
        }

        string username;
        do
        {
            username = $"user{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        }
        while (await context.Admins.AnyAsync(a => a.Username == username));

        string password = Convert.ToBase64String(RandomNumberGenerator.GetBytes(8));

        var adminProfile = new Admin()
        {
            Username = username,
            PasswordHash = password,
            TelegramUserId = user.Id
        };

        await context.Admins.AddAsync(adminProfile);
        await context.SaveChangesAsync();

        await bot.SendMessage(
            chatId: user.ChatId,
            text: user.Language switch
            {
                Language.Russian => $"📝 Ваш профиль успешно создан!\n\nИмя пользователя: `{username}`\nПароль: `{password}`",
                Language.Uzbek => $"📝 Profilingiz muvaffaqiyatli yaratildi!\n\nFoydalanuvchi nomi: `{username}`\nParol: `{password}`",
                _ => $"📝 Profilingiz muvaffaqiyatli yaratildi!\n\nFoydalanuvchi nomi: `{username}`\nParol: `{password}`"
            },
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );
    }

    public async Task HandleGetProfileAsync(Message message)
    {
        // Find the user
        var user = await context.TelegramUsers.FirstOrDefaultAsync(u => u.ChatId == message.Chat.Id);
        if (user is null) return;

        // Check if user is admin
        if (!user.IsAdmin)
        {
            await bot.SendMessage(
                chatId: user.ChatId,
                text: user.Language switch
                {
                    Language.Russian => "❌ У вас нет прав администратора.",
                    Language.Uzbek => "❌ Sizda admin huquqi mavjud emas.",
                    _ => "❌ You are not an admin."
                }
            );
            return;
        }

        // Get the admin profile
        var adminProfile = await context.Admins.FirstOrDefaultAsync(a => a.TelegramUserId == user.Id);
        if (adminProfile is null)
        {
            await bot.SendMessage(
                chatId: user.ChatId,
                text: user.Language switch
                {
                    Language.Russian => "📝 У вас нет профиля. Пожалуйста, создайте профиль с помощью кнопки «Создать профиль».",
                    Language.Uzbek => "📝 Sizda profil mavjud emas. Iltimos, «Profil yaratish» tugmasi orqali profil yarating.",
                    _ => "📝 You don't have a profile yet. Please create one using the 'Create profile' button."
                }
            );
            return;
        }

        // Send credentials
        await bot.SendMessage(
            chatId: user.ChatId,
            text: user.Language switch
            {
                Language.Russian => $"📝 Ваши учетные данные:\nUsername: `{adminProfile.Username}`\nPassword: `{adminProfile.PasswordHash}`",
                Language.Uzbek => $"📝 Profil ma'lumotlaringiz:\nUsername: `{adminProfile.Username}`\nParol: `{adminProfile.PasswordHash}`",
                _ => $"📝 Your profile credentials:\nUsername: `{adminProfile.Username}`\nPassword: `{adminProfile.PasswordHash}`"
            },
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
        );
    }
}
