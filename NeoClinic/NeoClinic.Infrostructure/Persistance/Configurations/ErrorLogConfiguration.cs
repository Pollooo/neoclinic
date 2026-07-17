using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Infrostructure.Persistance.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.ToTable("ErrorLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.StackTrace)
            .HasMaxLength(8000);

        builder.Property(x => x.Source)
            .HasMaxLength(500);

        builder.Property(x => x.Path)
            .HasMaxLength(500);

        builder.Property(x => x.Method)
            .HasMaxLength(50);

        builder.Property(x => x.IpAddress)
            .HasMaxLength(50);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(500);

        builder.Property(x => x.UserId)
            .HasMaxLength(100);

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.HasIndex(x => x.Timestamp);
    }
}
