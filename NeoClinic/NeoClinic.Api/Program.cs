using NeoClinic.Api;
using NeoClinic.Api.Endpoints;
using NeoClinic.Application;
using NeoClinic.Infrostructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHealthChecks("/health");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapEndpoints();

await app.RunAsync();
