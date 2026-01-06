using MediatR;
using Microsoft.EntityFrameworkCore;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.Doctors.GetDoctors;

public class GetDoctorsRequestHandler(
    IApplicationDbContext context)
    : IRequestHandler<GetDoctorsRequest, List<GetDoctorsResponse>>
{
    public async Task<List<GetDoctorsResponse>> Handle(GetDoctorsRequest request, CancellationToken cancellationToken)
    {
        return await context.Doctors
            .Where(f => !request.DoctorId.HasValue || request.DoctorId == f.Id).Select(
            d => new GetDoctorsResponse(
                d.Id,
                d.FullNameUz,
                d.SpecialtyUz,
                d.BioUz,
                d.FullNameRu,
                d.SpecialtyRu,
                d.BioRu,
                d.PhotoUrl))
            .ToListAsync(cancellationToken);
    }
}
