using NeoClinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NeoClinic.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<TelegramUser> TelegramUsers { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
