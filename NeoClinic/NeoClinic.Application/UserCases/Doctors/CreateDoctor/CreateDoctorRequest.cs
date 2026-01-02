using MediatR;
using Microsoft.AspNetCore.Http;

namespace NeoClinic.Application.UserCases.Doctors.CreateDoctor;

public record CreateDoctorRequest(
    string FullName,
    string Specialty,
    string Bio,
    IFormFile Photo)
    : IRequest<bool>;
