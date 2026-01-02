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
        return await context.Doctors.Select(
            d => new GetDoctorsResponse(
                d.Id,
                d.FullName,
                d.Specialty,
                d.PhotoUrl,
                d.Bio))
            .ToListAsync(cancellationToken);
    }
}
