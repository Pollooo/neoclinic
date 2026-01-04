namespace NeoClinic.Domain.Entities;

public class Service
{
    public Guid Id { get; set; }

    public string NameUz { get; set; } = null!;
    public string NameRu { get; set; } = null!;
    public string? DescriptionUz { get; set; }
    public string? DescriptionRu { get; set; }
    public decimal? Price { get; set; }
}
