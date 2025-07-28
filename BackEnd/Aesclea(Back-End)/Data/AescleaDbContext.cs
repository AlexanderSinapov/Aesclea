using Microsoft.EntityFrameworkCore;
using Aesclea_Back_End_.Models;

namespace Aesclea_Back_End_.Data
{
    public class AescleaDbContext : DbContext
    {
        public AescleaDbContext(DbContextOptions<AescleaDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Email).HasColumnName("Email").IsRequired();
                entity.Property(e => e.PasswordHash).HasColumnName("PasswordHash").IsRequired();
                entity.Property(e => e.FirstName).HasColumnName("FirstName").IsRequired();
                entity.Property(e => e.LastName).HasColumnName("LastName").IsRequired();
                entity.Property(e => e.Phone).HasColumnName("Phone").IsRequired();
                entity.Property(e => e.Role).HasColumnName("Role").IsRequired();
                entity.Property(e => e.MedicalNumber).HasColumnName("MedicalNumber").IsRequired();
                entity.Property(e => e.Hospital).HasColumnName("Hospital").IsRequired();
                entity.Property(e => e.EmailVerified).HasColumnName("EmailVerified").HasDefaultValue(false);
                entity.Property(e => e.EmailVerificationToken).HasColumnName("EmailVerificationToken");
                entity.Property(e => e.EmailVerificationTokenExpiry).HasColumnName("EmailVerificationTokenExpiry");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                // Create unique index on email
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configure Patient entity
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.ToTable("Patients");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.FirstName).HasColumnName("FirstName").IsRequired();
                entity.Property(e => e.LastName).HasColumnName("LastName").IsRequired();
                entity.Property(e => e.Email).HasColumnName("Email");
                entity.Property(e => e.Phone).HasColumnName("Phone");
                entity.Property(e => e.DateOfBirth).HasColumnName("DateOfBirth");
                entity.Property(e => e.Gender).HasColumnName("Gender");
                entity.Property(e => e.MedicalHistory).HasColumnName("MedicalHistory");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
            });
        }
    }
}
