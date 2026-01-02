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

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Specialty)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(x => x.Bio)
            .IsRequired(false)
            .HasMaxLength(500);
    }
}
