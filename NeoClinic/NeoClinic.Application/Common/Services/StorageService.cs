using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.Common.Services;

public class StorageService : IStorageService
{
    private readonly BlobContainerClient _containerClient;

    public StorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"];
        var containerName = configuration["AzureBlob:ContainerName"];

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);
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
        var blobClient = _containerClient.GetBlobClient(blobName);
        content.Seek(0, SeekOrigin.Begin);
        await blobClient.UploadAsync(content, overwrite: true);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.DeleteIfExistsAsync();
        return response.Value;
    }

    public Task<string> GetPublicUrl(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        return Task.FromResult(blobClient.Uri.ToString());
    }

    public async Task<(byte[] Content, string ContentType)> GetFileBytesAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var response = await blobClient.DownloadContentAsync();
        return (response.Value.Content.ToArray(), response.Value.Details.ContentType);
    }
}
