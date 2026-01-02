using Microsoft.EntityFrameworkCore;
using NeoClinic.Domain.Entities;

namespace NeoClinic.Infrostructure.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Admin> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("clinic");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
