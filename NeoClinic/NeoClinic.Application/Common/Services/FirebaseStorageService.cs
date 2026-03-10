using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.Common.Services;

public class FirebaseStorageService : IStorageService
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public FirebaseStorageService(IConfiguration configuration)
    {
        var credentialsPath = configuration["Firebase:CredentialsPath"];
        _bucketName = configuration["Firebase:BucketName"] ?? throw new ArgumentNullException("Firebase:BucketName configuration is missing");

        if (string.IsNullOrEmpty(credentialsPath))
        {
            throw new ArgumentNullException("Firebase:CredentialsPath configuration is missing");
        }

        var credential = GoogleCredential.FromFile(credentialsPath);
        _storageClient = StorageClient.Create(credential);
    }

    public async Task<string> UploadFileAsync(string blobName, Stream content)
    {
        try
        {
            content.Seek(0, SeekOrigin.Begin);

            var uploadOptions = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            };

            await _storageClient.UploadObjectAsync(
                _bucketName,
                blobName,
                "application/octet-stream",
                content,
                uploadOptions);

            var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{blobName}";
            return publicUrl;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error uploading file to Firebase Storage: {ex.Message}", ex);
        }
    }

    public async Task<bool> DeleteFileAsync(string blobName)
    {
        try
        {
            await _storageClient.DeleteObjectAsync(_bucketName, blobName);
            return true;
        }
        catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting file from Firebase Storage: {ex.Message}", ex);
        }
    }

    public Task<string> GetPublicUrl(string blobName)
    {
        try
        {
            var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{blobName}";
            return Task.FromResult(publicUrl);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating public URL: {ex.Message}", ex);
        }
    }

    public string GenerateBlobName(MediaType mediaType, string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";
        var folder = mediaType == MediaType.Image ? "images" : "videos";
        return $"{folder}/{uniqueName}";
    }
}
