using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoClinic.Domain.Entities;
using System.Runtime.CompilerServices;

namespace NeoClinic.Infrostructure.Persistance.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullNameRu)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(x => x.FullNameUz)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SpecialtyUz)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.BioUz)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(x => x.SpecialtyRu)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.BioRu)
            .IsRequired(false)
            .HasMaxLength(500);
    }
}
