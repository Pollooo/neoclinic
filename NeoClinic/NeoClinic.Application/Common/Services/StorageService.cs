using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.Common.Services;

public class StorageService : IStorageService
{
    public Task<bool> DeleteFileAsync(string blobName)
    {
        throw new NotImplementedException();
    }

    public string GenerateBlobName(MediaType mediaType, string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetPublicUrl(string blobName)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadFileAsync(string blobName, Stream content)
    {
        throw new NotImplementedException();
    }
}
