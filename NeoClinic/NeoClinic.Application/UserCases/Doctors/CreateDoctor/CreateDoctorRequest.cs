using MediatR;
using Microsoft.AspNetCore.Http;

namespace NeoClinic.Application.UserCases.Doctors.CreateDoctor;

public record CreateDoctorRequest(
    string FullNameUz,
    string BioUz,
    string SpecialtyUz,
    string FullNameRu,
    string BioRu,
    string SpecialtyRu,
    IFormFile Photo)
    : IRequest<bool>;
