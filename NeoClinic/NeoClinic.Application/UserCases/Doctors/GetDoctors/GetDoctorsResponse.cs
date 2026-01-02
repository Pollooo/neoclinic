namespace NeoClinic.Application.UserCases.Doctors.GetDoctors;

public record GetDoctorsResponse(
    Guid DoctorId,
    string FullName,
    string? Specialty,
    string? PhotoUrl,
    string? Bio);
