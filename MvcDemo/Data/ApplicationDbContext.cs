using Microsoft.EntityFrameworkCore;
using MvcDemo.Models;

namespace MvcDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Índice para búsquedas por rango
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => a.Start);

            // (Opcional) evita dos citas que inicien exactamente igual
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.Start })
                .IsUnique(false);
        }
    }
}