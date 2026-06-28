using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using NeoClinic.Domain.Enums;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctorPhoto;

public class UpdateDoctorPhotoRequestHandler(
    IApplicationDbContext context,
    IStorageService storageService)
    : IRequestHandler<UpdateDoctorPhotoRequest, bool>
{
    public async Task<bool> Handle(UpdateDoctorPhotoRequest request, CancellationToken cancellationToken)
    {
        var doctor = await context.Doctors
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

        if (doctor is null)
            return false;

        var oldBlobName = doctor.BlobName;
        var oldPhotoUrl = doctor.PhotoUrl;
        var fileName = request.Photo.FileName;
        var blobName = storageService.GenerateBlobName(MediaType.Image, fileName);
        var photoUrl = await storageService.UploadFileAsync(blobName, request.Photo.OpenReadStream());

        doctor.PhotoUrl = photoUrl;
        doctor.BlobName = blobName;

        var oldMediaFile = await context.MediaFiles
            .FirstOrDefaultAsync(f => f.FileUrl == oldPhotoUrl || f.BlobName == oldBlobName, cancellationToken);

        if (oldMediaFile is not null)
        {
            oldMediaFile.FileName = fileName;
            oldMediaFile.BlobName = blobName;
            oldMediaFile.FileSizeInBytes = request.Photo.Length;
            oldMediaFile.FileUrl = photoUrl;
            oldMediaFile.ContentType = request.Photo.ContentType;
            oldMediaFile.IsDoctor = true;
            oldMediaFile.Type = MediaType.Image;
        }
        else
        {
            await context.MediaFiles.AddAsync(new MediaFile
            {
                FileName = fileName,
                BlobName = blobName,
                FileSizeInBytes = request.Photo.Length,
                FileDescriptionUz = "Photo of a doctor",
                FileUrl = photoUrl,
                ContainerName = "neo-clinic-docs",
                ContentType = request.Photo.ContentType,
                IsDoctor = true,
                Type = MediaType.Image,
            }, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldBlobName))
        {
            try
            {
                await storageService.DeleteFileAsync(oldBlobName);
            }
            catch
            {
                // Old photo might be in a different storage provider — skip
            }
        }

        return true;
    }
}
