namespace NeoClinic.Application.UserCases.Doctors.GetDoctors;

public record GetDoctorsResponse(
    Guid DoctorId,
    string FullNameUz,
    string? SpecialtyUz,
    string? BioUz,
    string FullNameRu,
    string? SpecialtyRu,
    string? BioRu,
    string? PhotoUrl);
