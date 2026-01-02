using MediatR;
using Microsoft.AspNetCore.Http;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctor;

public record UpdateDoctorRequest(
    Guid DoctorId,
    string FullName,
    string Specialty,
    string Bio,
    IFormFile Photo)
    : IRequest<bool>;
