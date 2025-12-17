using Fitness.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fitness.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Salon> Salonlar { get; set; }
        public DbSet<Antrenor> Antrenorler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }
        public DbSet<AntrenorHizmet> AntrenorHizmetler { get; set; }
        public DbSet<AntrenorMusaitlik> AntrenorMusaitlikler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many: Antrenor-Hizmet composite key
            modelBuilder.Entity<AntrenorHizmet>()
                .HasKey(ah => new { ah.AntrenorId, ah.HizmetId });

            modelBuilder.Entity<AntrenorHizmet>()
                .HasOne(ah => ah.Antrenor)
                .WithMany(a => a.AntrenorHizmetler)
                .HasForeignKey(ah => ah.AntrenorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AntrenorHizmet>()
                .HasOne(ah => ah.Hizmet)
                .WithMany(h => h.AntrenorHizmetler)
                .HasForeignKey(ah => ah.HizmetId)
                .OnDelete(DeleteBehavior.Restrict);

            // Randevu ilişkileri
            modelBuilder.Entity<Randevu>()
                .HasOne(r => r.Uye)
                .WithMany(u => u.Randevular)
                .HasForeignKey(r => r.UyeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Randevu>()
                .HasOne(r => r.Antrenor)
                .WithMany(a => a.Randevular)
                .HasForeignKey(r => r.AntrenorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
