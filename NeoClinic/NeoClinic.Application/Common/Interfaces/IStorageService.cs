using NeoClinic.Domain.Enums;
using System.Xml.Linq;

namespace NeoClinic.Application.Common.Interfaces;

public interface IStorageService
{
    string GenerateBlobName(MediaType mediaType, string fileName);
    Task<string> UploadFileAsync(string blobName, Stream content);
    Task<bool> DeleteFileAsync(string blobName);
    Task<string> GetPublicUrl(string blobName);
}
