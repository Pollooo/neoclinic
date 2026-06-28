using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.Common.Services;

public class SupabaseStorageService : IStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _url;
    private readonly string _serviceRoleKey;
    private readonly string _bucketName;

    private void AddAuthHeaders(HttpRequestMessage request)
    {
        request.Headers.Add("apikey", _serviceRoleKey);
        request.Headers.Add("Authorization", $"Bearer {_serviceRoleKey}");
    }

    public SupabaseStorageService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _url = (configuration["Supabase:Url"] ?? throw new ArgumentNullException("Supabase:Url configuration is missing")).TrimEnd('/');
        _serviceRoleKey = configuration["Supabase:ServiceRoleKey"] ?? throw new ArgumentNullException("Supabase:ServiceRoleKey configuration is missing");
        _bucketName = configuration["Supabase:BucketName"] ?? throw new ArgumentNullException("Supabase:BucketName configuration is missing");
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
            var fileId = GetFileId(blobName);
            var requestUri = $"{_url}/storage/v1/object/{_bucketName}/{fileId}";

            if (content.CanSeek)
                content.Seek(0, SeekOrigin.Begin);

            using var memoryStream = new MemoryStream();
            await content.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var httpClient = _httpClientFactory.CreateClient("Supabase");
            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            AddAuthHeaders(request);
            request.Headers.Add("x-upsert", "true");
            request.Content = new ByteArrayContent(fileBytes);
            request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(GetContentType(blobName));

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Supabase upload failed with status {response.StatusCode}: {error}");
            }

            return GetPublicUrl(blobName).Result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file to Supabase Storage: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        try
        {
            var fileId = GetFileId(blobName);
            var requestUri = $"{_url}/storage/v1/object/{_bucketName}/{fileId}";

            var httpClient = _httpClientFactory.CreateClient("Supabase");
            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            AddAuthHeaders(request);

            var response = await httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Supabase delete failed with status {response.StatusCode}: {error}");
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting file from Supabase Storage: {ex.Message}", ex);
        }
    }

    public Task<string> GetPublicUrl(string blobName)
    {
        var fileId = GetFileId(blobName);
        var publicUrl = $"{_url}/storage/v1/object/public/{_bucketName}/{fileId}";
        return Task.FromResult(publicUrl);
    }

    public async Task<(byte[] Content, string ContentType)> GetFileBytesAsync(string blobName)
    {
        var fileId = GetFileId(blobName);
        var requestUri = $"{_url}/storage/v1/object/{_bucketName}/{fileId}";

        var httpClient = _httpClientFactory.CreateClient("Supabase");
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        AddAuthHeaders(request);

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        return (bytes, contentType);
    }

    private string GetFileId(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
            throw new ArgumentException("Blob name cannot be null or empty", nameof(blobName));

        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(blobName);
        if (string.IsNullOrEmpty(fileNameWithoutExt))
            fileNameWithoutExt = Path.GetFileName(blobName);

        return fileNameWithoutExt.Replace("/", "-").Replace("\\", "-");
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
}
