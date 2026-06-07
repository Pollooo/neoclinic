using MediatR;
using Microsoft.AspNetCore.Http;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctorPhoto;

public record UpdateDoctorPhotoRequest(
    Guid DoctorId,
    IFormFile Photo)
    : IRequest<bool>;
