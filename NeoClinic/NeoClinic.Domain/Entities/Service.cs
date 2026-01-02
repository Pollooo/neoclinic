namespace NeoClinic.Domain.Entities;

public class Service
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}
