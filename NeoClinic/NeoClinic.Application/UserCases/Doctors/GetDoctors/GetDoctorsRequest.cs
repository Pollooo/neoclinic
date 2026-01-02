using MediatR;

namespace NeoClinic.Application.UserCases.Doctors.GetDoctors;

public record GetDoctorsRequest() : IRequest<List<GetDoctorsResponse>>;
