using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Infrostructure.Persistance.Configurations;

public class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("MediaFiles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.AltText)
            .HasMaxLength(300);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
