using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Infrostructure.Persistance.Configurations;

public class TelegramFileMapConfiguration : IEntityTypeConfiguration<TelegramFileMap>
{
    public void Configure(EntityTypeBuilder<TelegramFileMap> builder)
    {
        builder.ToTable("TelegramFileMaps");

        builder.HasKey(x => x.BlobName);

        builder.Property(x => x.BlobName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.FileId)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
