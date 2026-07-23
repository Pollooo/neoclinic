using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.Common.Services;

public class TelegramStorageService : IStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IApplicationDbContext _dbContext;
    private readonly string _botToken;
    private readonly long _chatId;

    public TelegramStorageService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IApplicationDbContext dbContext)
    {
        _httpClientFactory = httpClientFactory;
        _dbContext = dbContext;
        _botToken = configuration["TelegramStorage:BotToken"] ?? throw new ArgumentNullException("TelegramStorage:BotToken");
        _chatId = long.Parse(configuration["TelegramStorage:ChatId"] ?? throw new ArgumentNullException("TelegramStorage:ChatId"));
    }

    public string GenerateBlobName(MediaType mediaType, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";
        var folder = mediaType == MediaType.Image ? "images" : "videos";
        return $"{folder}/{uniqueName}";
    }

    public async Task<string> UploadFileAsync(string blobName, Stream content)
    {
        try
        {
            if (content.CanSeek)
                content.Seek(0, SeekOrigin.Begin);

            var httpClient = _httpClientFactory.CreateClient("TelegramStorage");
            var isVideo = blobName.Contains("/videos/");

            using var request = new HttpRequestMessage(HttpMethod.Post,
                $"https://api.telegram.org/bot{_botToken}/{(isVideo ? "sendVideo" : "sendDocument")}");
            request.Headers.Add("Accept", "application/json");

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(_chatId.ToString()), "chat_id");

            var fileName = Path.GetFileName(blobName);
            if (string.IsNullOrEmpty(fileName))
                fileName = "file";

            var streamContent = new StreamContent(content);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(blobName));
            multipartContent.Add(streamContent, isVideo ? "video" : "document", fileName);

            request.Content = multipartContent;

            using var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Telegram upload failed ({(int)response.StatusCode}): {TruncateJson(responseBody, 200)}");
            }

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var result = jsonDoc.RootElement.GetProperty("result");

            var fileId = ExtractFileId(result, isVideo)
                ?? throw new Exception("Telegram response missing file_id");

            var contentType = GetContentType(blobName);

            _dbContext.TelegramFileMaps.Add(new TelegramFileMap
            {
                BlobName = blobName,
                FileId = fileId,
                ContentType = contentType,
                CreatedAt = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync();

            return $"tg://file/{fileId}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file to Telegram Storage: {ex.Message}", ex);
        }
    }

    private static string? ExtractFileId(JsonElement result, bool isVideo)
    {
        if (isVideo && result.TryGetProperty("video", out var video) && video.TryGetProperty("file_id", out var vidId))
            return vidId.GetString();

        if (result.TryGetProperty("document", out var doc) && doc.TryGetProperty("file_id", out var docId))
            return docId.GetString();

        if (result.TryGetProperty("video", out var vid) && vid.TryGetProperty("file_id", out var vidId2))
            return vidId2.GetString();

        return null;
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        try
        {
            var mapping = await _dbContext.TelegramFileMaps
                .FirstOrDefaultAsync(m => m.BlobName == blobName);

            if (mapping is not null)
            {
                _dbContext.TelegramFileMaps.Remove(mapping);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> GetPublicUrl(string blobName)
    {
        var mapping = await _dbContext.TelegramFileMaps
            .FirstOrDefaultAsync(m => m.BlobName == blobName);

        if (mapping is not null)
            return $"tg://file/{mapping.FileId}";

        return blobName;
    }

    public async Task<(byte[] Content, string ContentType)> GetFileBytesAsync(string blobName)
    {
        var mapping = await _dbContext.TelegramFileMaps
            .FirstOrDefaultAsync(m => m.BlobName == blobName);

        if (mapping is null)
            return ([], "application/octet-stream");

        try
        {
            var httpClient = _httpClientFactory.CreateClient("TelegramStorage");

            var getFileUrl = $"https://api.telegram.org/bot{_botToken}/getFile?file_id={mapping.FileId}";
            using var getFileResponse = await httpClient.GetAsync(getFileUrl);
            getFileResponse.EnsureSuccessStatusCode();
            var getFileBody = await getFileResponse.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(getFileBody);
            var filePath = jsonDoc.RootElement.GetProperty("result").GetProperty("file_path").GetString()
                ?? throw new Exception("Telegram response missing file_path");

            var downloadUrl = $"https://api.telegram.org/file/bot{_botToken}/{filePath}";
            using var downloadResponse = await httpClient.GetAsync(downloadUrl);
            downloadResponse.EnsureSuccessStatusCode();

            var bytes = await downloadResponse.Content.ReadAsByteArrayAsync();
            var contentType = downloadResponse.Content.Headers.ContentType?.MediaType
                ?? mapping.ContentType
                ?? "application/octet-stream";

            return (bytes, contentType);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error downloading file from Telegram Storage: {ex.Message}", ex);
        }
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            _ => "application/octet-stream"
        };
    }

    private static string TruncateJson(string json, int maxLen)
    {
        return json.Length <= maxLen ? json : json[..maxLen] + "...";
    }
}
