using MediatR;

namespace NeoClinic.Application.UserCases.Doctors.UpdateDoctor;

public record UpdateDoctorRequest(
    Guid DoctorId,
    string FullNameUz,
    string BioUz,
    string SpecialtyUz,
    string FullNameRu,
    string BioRu,
    string SpecialtyRu)
    : IRequest<bool>;
