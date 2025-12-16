using Microsoft.EntityFrameworkCore;
using ApiForPractik.Models;

namespace ApiForPractik.Data
{
    public class RequestContext : DbContext
    {
        public RequestContext(DbContextOptions<RequestContext> options) : base(options)
        {
        }

        public DbSet<ApplicationRequest> ApplicationRequests { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<StatusHistory> StatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).IsRequired();
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Priority).HasMaxLength(20);
                entity.Property(e => e.DateCreated).IsRequired();
                entity.Property(e => e.DateUpdated).IsRequired();

                // Настройка связи с вложениями
                entity.HasMany(r => r.Attachments)
                      .WithOne(a => a.ApplicationRequest)
                      .HasForeignKey(a => a.ApplicationRequestId);

                // Настройка связи с историей статусов
                entity.HasMany(r => r.StatusHistory)
                      .WithOne(s => s.ApplicationRequest)
                      .HasForeignKey(s => s.ApplicationRequestId);
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.FilePath).IsRequired();
            });

            modelBuilder.Entity<StatusHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PreviousStatus).IsRequired();
                entity.Property(e => e.NewStatus).IsRequired();
                entity.Property(e => e.ChangeDate).IsRequired();
            });
        }
    }
}