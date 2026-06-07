using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctor;

public class UpdateDoctorRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<UpdateDoctorRequest, bool>
{
    public async Task<bool> Handle(UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        var doctor = await context.Doctors
            .FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);

        if (doctor is null)
            return false;

        doctor.FullNameUz = request.FullNameUz;
        doctor.BioUz = request.BioUz;
        doctor.SpecialtyUz = request.SpecialtyUz;
        doctor.FullNameRu = request.FullNameRu;
        doctor.BioRu = request.BioRu;
        doctor.SpecialtyRu = request.SpecialtyRu;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
