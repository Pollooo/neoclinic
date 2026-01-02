using MediatR;

namespace NeoClinic.Application.UserCases.Doctors.DeleteDoctor;

public record DeleteDoctorRequest(Guid DoctorId) : IRequest<bool>;
