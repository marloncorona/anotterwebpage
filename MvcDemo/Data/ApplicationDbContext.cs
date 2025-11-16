using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcDemo.Models;

namespace MvcDemo.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.Property(a => a.Start).IsRequired();
                entity.Property(a => a.End).IsRequired();
                entity.Property(a => a.Nombre).IsRequired();
                entity.Property(a => a.Apellido).IsRequired();
                entity.Property(a => a.Email).IsRequired();
                entity.Property(a => a.Numero).IsRequired();
                entity.Property(a => a.Motivo).IsRequired();
                entity.Property(a => a.isReserved).HasDefaultValue(true);
            });
        }
    }
}