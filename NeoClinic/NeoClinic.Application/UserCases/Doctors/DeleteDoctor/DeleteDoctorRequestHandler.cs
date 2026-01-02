using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;
using System.Text.Json;

namespace NeoClinic.Application.UserCases.Doctors.DeleteDoctor;

public class DeleteDoctorRequestHandler(
    IApplicationDbContext context,
    ITelegramBotService botService,
    IStorageService storageService) : IRequestHandler<DeleteDoctorRequest, bool>
{
    public async Task<bool> Handle(DeleteDoctorRequest request, CancellationToken cancellationToken)
    {
        var doctor = await context.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);
        if (doctor is null)
            return false;

        context.Doctors.Remove(doctor);
        if (!await storageService.DeleteFileAsync(doctor.BlobName))
            return false;

        var file = await context.MediaFiles.FirstOrDefaultAsync(f => f.FileUrl == doctor.PhotoUrl, cancellationToken);
        if (file is null)
            return false;

        context.MediaFiles.Remove(file);
        if (await context.SaveChangesAsync(cancellationToken) > 1)
            return true;

        await botService.NotifyAboutErrorAsync(JsonSerializer.Serialize(request), "DeleteDoctorRequestHandler");
        return false;
    }
}
