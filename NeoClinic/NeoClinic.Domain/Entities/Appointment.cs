namespace NeoClinic.Domain.Entities;

public class Appointment
{
    public Guid Id { get; set; }

    public string PatientName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }

    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public DateTime AppointmentDate { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
