using EventDrivenCQRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventDrivenCQRS.Infrastructure.Persistence
{
    public class EventDrivenCQRSDbContext : DbContext
    {
        public EventDrivenCQRSDbContext(DbContextOptions<EventDrivenCQRSDbContext> options)
            : base(options)
        {
        }

        // 📌 DbSet Tanımları (Tablolar)
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 📌 Entity Configuration (Opsiyonel)
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            });
        }
    }
}