using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Infrostructure.Persistance.Configurations;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("ContactMessages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasMaxLength(100);

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(x => x.TelegramChatUrl)
            .HasMaxLength(300);

        builder.Property(x => x.TelegramUrl)
            .HasMaxLength(300);

        builder.Property(x => x.InstagramUrl)
            .HasMaxLength(300);

        builder.Property(x => x.FacebookUrl)
            .HasMaxLength(300);

        builder.Property(x => x.LocationUrl)
            .HasMaxLength(300);

        builder.Property(x => x.AboutClinic)
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}
