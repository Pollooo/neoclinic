using MediatR;

namespace NeoClinic.Application.UserCases.Doctors.GetDoctors;

public record GetDoctorsRequest(Guid? DoctorId) : IRequest<List<GetDoctorsResponse>>;
