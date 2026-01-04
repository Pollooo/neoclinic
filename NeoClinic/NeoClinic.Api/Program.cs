using NeoClinic.Api;
using NeoClinic.Api.Endpoints;
using NeoClinic.Application;
using NeoClinic.Application.Common.Services.TelegramBotService.UpdateHandler;
using NeoClinic.Infrostructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddAntiforgery();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

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

app.MapEndpoints();

await app.RunAsync();
