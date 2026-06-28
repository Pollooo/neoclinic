using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;
using System.Net;
using System.Net.Http.Headers;

namespace NeoClinic.Application.Common.Services;

public class AppwriteStorageService : IStorageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _endpoint;
    private readonly string _projectId;
    private readonly string _apiKey;
    private readonly string _bucketId;

    public AppwriteStorageService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _endpoint = configuration["Appwrite:Endpoint"] ?? throw new ArgumentNullException("Appwrite:Endpoint configuration is missing");
        _projectId = configuration["Appwrite:ProjectId"] ?? throw new ArgumentNullException("Appwrite:ProjectId configuration is missing");
        _apiKey = configuration["Appwrite:ApiKey"] ?? throw new ArgumentNullException("Appwrite:ApiKey configuration is missing");
        _bucketId = configuration["Appwrite:BucketId"] ?? throw new ArgumentNullException("Appwrite:BucketId configuration is missing");

        if (_endpoint.EndsWith("/"))
        {
            _endpoint = _endpoint.Substring(0, _endpoint.Length - 1);
        }
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
            var fileName = Path.GetFileName(blobName);

            byte[] fileBytes;
            if (content.CanSeek)
            {
                content.Seek(0, SeekOrigin.Begin);
            }
            using (var ms = new MemoryStream())
            {
                await content.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            var requestUri = $"{_endpoint}/storage/buckets/{_bucketId}/files";

            var httpClient = _httpClientFactory.CreateClient("Appwrite");

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Version = HttpVersion.Version11,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrLower
            };
            request.Headers.ExpectContinue = false;
            request.Headers.Add("X-Appwrite-Project", _projectId);
            request.Headers.Add("X-Appwrite-Key", _apiKey);

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(fileId), "fileId");
            multipartContent.Add(new StringContent("read(\"any\")"), "permissions[]");

            var streamContent = new ByteArrayContent(fileBytes);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(GetContentType(fileName));
            multipartContent.Add(streamContent, "file", fileName);


            request.Content = multipartContent;

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Appwrite upload failed with status code {response.StatusCode}: {errorResponse}");
            }

            return await GetPublicUrl(blobName);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file to Appwrite Storage: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        try
        {
            var fileId = GetFileId(blobName);
            var requestUri = $"{_endpoint}/storage/buckets/{_bucketId}/files/{fileId}";

            var httpClient = _httpClientFactory.CreateClient("Appwrite");

            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            request.Headers.Add("X-Appwrite-Project", _projectId);
            request.Headers.Add("X-Appwrite-Key", _apiKey);

            var response = await httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"Appwrite delete failed with status code {response.StatusCode}: {errorResponse}");
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting file from Appwrite Storage: {ex.Message}", ex);
        }
    }

    public Task<string> GetPublicUrl(string blobName)
    {
        try
        {
            var fileId = GetFileId(blobName);
            var publicUrl = $"{_endpoint}/storage/buckets/{_bucketId}/files/{fileId}/view?project={_projectId}";
            return Task.FromResult(publicUrl);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating public URL: {ex.Message}", ex);
        }
    }

    public async Task<(byte[] Content, string ContentType)> GetFileBytesAsync(string blobName)
    {
        var fileId = GetFileId(blobName);
        var requestUri = $"{_endpoint}/storage/buckets/{_bucketId}/files/{fileId}/view?project={_projectId}";

        var httpClient = _httpClientFactory.CreateClient("Appwrite");
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("X-Appwrite-Project", _projectId);
        request.Headers.Add("X-Appwrite-Key", _apiKey);

        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync();
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        return (bytes, contentType);
    }

    private string GetFileId(string blobName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new ArgumentException("Blob name cannot be null or empty", nameof(blobName));
        }

        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(blobName);
        if (string.IsNullOrEmpty(fileNameWithoutExt))
        {
            fileNameWithoutExt = Path.GetFileName(blobName);
        }

        // Replace any potential invalid characters just in case
        var fileId = fileNameWithoutExt.Replace("/", "-").Replace("\\", "-");
        if (fileId.Length > 36)
        {
            fileId = fileId.Substring(0, 36);
        }

        return fileId;
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
}
