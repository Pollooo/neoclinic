using NeoClinic.Api;
using NeoClinic.Api.Endpoints;
using NeoClinic.Application;
using NeoClinic.Infrostructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddAntiforgery();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});
builder.Services.AddClinicCors();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.ApplyMigrations();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");
app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.StartTelegramBot();

app.MapEndpoints();

await app.RunAsync();
