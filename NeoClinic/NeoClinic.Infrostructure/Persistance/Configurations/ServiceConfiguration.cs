using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Infrostructure.Persistance.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NameUz)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DescriptionUz)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.NameRu)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DescriptionRu)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.Price)
            .IsRequired(false)
            .HasColumnType("decimal(18,2)");
    }
}
