using MediatR;
using NeoClinic.Application.Common.Interfaces;
using NeoClinic.Domain.Entities;
using NeoClinic.Domain.Enums;
using System.Text.Json;

namespace NeoClinic.Application.UserCases.Doctors.CreateDoctor;

public class CreateDoctorRequestHandler(
    IApplicationDbContext context,
    IStorageService storageService,
    ITelegramBotService botService)
    : IRequestHandler<CreateDoctorRequest, bool>
{
    public async Task<bool> Handle(CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var fileName = request.Photo.FileName;
        var blobName = storageService.GenerateBlobName(MediaType.Image, fileName);
        var photeUrl = await storageService.UploadFileAsync(blobName, request.Photo.OpenReadStream());

        var doctor = new Doctor()
        {
            FullName = request.FullName,
            Bio = request.Bio,
            Specialty = request.Specialty,
            PhotoUrl = photeUrl,
            BlobName = blobName,
        };

        var document = new MediaFile()
        {
            FileName = fileName,
            BlobName = blobName,
            FileSizeInBytes = request.Photo.Length,
            FileDescription = "Photo of a doctor",
            FileUrl = photeUrl,
            ContainerName = "neo-clinic-docs",
            ContentType = request.Photo.ContentType,
            IsDoctor = true,
            Type = MediaType.Image,
        };

        await context.Doctors.AddAsync(doctor, cancellationToken);
        await context.MediaFiles.AddAsync(document, cancellationToken);
            if (await context.SaveChangesAsync(cancellationToken) > 1)
                return true;

            await botService.NotifyAboutErrorAsync(JsonSerializer.Serialize(request), "CreateDoctorRequestHandler");
            return false;
    }
}
