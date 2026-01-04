using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Infrostructure.Persistance.Configurations;

public class TelegramUserConfiguration : IEntityTypeConfiguration<TelegramUser>
{
    public void Configure(EntityTypeBuilder<TelegramUser> builder)
    {
        builder.ToTable("TelegramUsers", "clinic");

        builder.HasKey(tu => tu.Id);
        builder.Property(tu => tu.Id)
            .ValueGeneratedOnAdd();

        builder.Property(tu => tu.Language)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
    }
}
