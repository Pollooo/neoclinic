using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.Common.Services;

public class StorageService : IStorageService
{
    private readonly BlobContainerClient Container;

    public StorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"];
        var containerName = configuration["AzureBlob:ContainerName"];

        var serviceClient = new BlobServiceClient(connectionString);
        Container = serviceClient.GetBlobContainerClient(containerName);
        Container.CreateIfNotExists();
    }

    public async Task<string> UploadFileAsync(string blobName, Stream content)
    {
        var blobClient = Container.GetBlobClient(blobName);
        await blobClient.UploadAsync(content, overwrite: true);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        var blobClient = Container.GetBlobClient(blobName);
        var response = await blobClient.DeleteIfExistsAsync();
        return response.Value;
    }

    public Task<string> GetPublicUrl(string blobName)
    {
        var blobClient = Container.GetBlobClient(blobName);
        return Task.FromResult(blobClient.Uri.ToString());
    }

    public string GenerateBlobName(MediaType mediaType, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";
        var folder = mediaType == MediaType.Image ? "images" : "videos";
        return $"{folder}/{uniqueName}";
    }
}
