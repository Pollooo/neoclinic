using MediatR;
using NeoClinic.Application.Common.Interfaces;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctor;

public class UpdateDoctorRequestHandler()
    : IRequestHandler<UpdateDoctorRequest, bool>
{
    public Task<bool> Handle(UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
