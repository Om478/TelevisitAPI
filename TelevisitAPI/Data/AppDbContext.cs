using Microsoft.EntityFrameworkCore;
using TelevisitAPI.Models;

namespace TelevisitAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Make PhoneNumber unique for simplicity
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.PhoneNumber)
                .IsUnique();
        }
    }
}
