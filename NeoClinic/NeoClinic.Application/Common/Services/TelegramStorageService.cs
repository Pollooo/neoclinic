using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.Common.Services;

public class TelegramStorageService : IStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _botToken;
    private readonly long _chatId;
    private readonly string _mappingFilePath;

    private Dictionary<string, TelegramFileMapping> _mappings = [];
    private readonly object _lock = new();

    public TelegramStorageService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _botToken = configuration["TelegramStorage:BotToken"] ?? throw new ArgumentNullException("TelegramStorage:BotToken");
        _chatId = long.Parse(configuration["TelegramStorage:ChatId"] ?? throw new ArgumentNullException("TelegramStorage:ChatId"));

            _mappingFilePath = Path.Combine(AppContext.BaseDirectory, "telegram-mappings.json");
        LoadMappings();
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

            using var memoryStream = new MemoryStream();
            await content.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var httpClient = _httpClientFactory.CreateClient("TelegramStorage");

            using var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.telegram.org/bot{_botToken}/sendDocument");
            request.Headers.Add("Accept", "application/json");

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(_chatId.ToString()), "chat_id");

            var fileName = Path.GetFileName(blobName);
            if (string.IsNullOrEmpty(fileName))
                fileName = "file";

            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(blobName));
            multipartContent.Add(fileContent, "document", fileName);

            request.Content = multipartContent;

            using var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Telegram upload failed ({(int)response.StatusCode}): {TruncateJson(responseBody, 200)}");
            }

            using var jsonDoc = JsonDocument.Parse(responseBody);
            var fileId = jsonDoc.RootElement.GetProperty("result").GetProperty("document").GetProperty("file_id").GetString()
                ?? throw new Exception("Telegram response missing file_id");

            SaveMapping(blobName, fileId, GetContentType(blobName));

            return $"tg://file/{fileId}";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file to Telegram Storage: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        try
        {
            RemoveMapping(blobName);
            return await Task.FromResult(true);
        }
        catch
        {
            return false;
        }
    }

    public Task<string> GetPublicUrl(string blobName)
    {
        var mapping = GetMapping(blobName);
        if (mapping is not null)
        {
            return Task.FromResult($"tg://file/{mapping.FileId}");
        }

        return Task.FromResult(blobName);
    }

    public async Task<(byte[] Content, string ContentType)> GetFileBytesAsync(string blobName)
    {
        var mapping = GetMapping(blobName);
        if (mapping is null)
        {
            return ([], "application/octet-stream");
        }

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

    private string GetContentType(string fileName)
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

    private void LoadMappings()
    {
        try
        {
            if (File.Exists(_mappingFilePath))
            {
                var json = File.ReadAllText(_mappingFilePath);
                _mappings = JsonSerializer.Deserialize<Dictionary<string, TelegramFileMapping>>(json) ?? [];
            }
        }
        catch
        {
            _mappings = [];
        }
    }

    private void SaveMappings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_mappings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_mappingFilePath, json);
        }
        catch
        {
        }
    }

    private void SaveMapping(string blobName, string fileId, string contentType)
    {
        lock (_lock)
        {
            _mappings[blobName] = new TelegramFileMapping
            {
                FileId = fileId,
                ContentType = contentType
            };
            SaveMappings();
        }
    }

    private void RemoveMapping(string blobName)
    {
        lock (_lock)
        {
            _mappings.Remove(blobName);
            SaveMappings();
        }
    }

    private TelegramFileMapping? GetMapping(string blobName)
    {
        lock (_lock)
        {
            _mappings.TryGetValue(blobName, out var mapping);
            return mapping;
        }
    }

    private static string TruncateJson(string json, int maxLen)
    {
        return json.Length <= maxLen ? json : json[..maxLen] + "...";
    }

    private class TelegramFileMapping
    {
        public string FileId { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
