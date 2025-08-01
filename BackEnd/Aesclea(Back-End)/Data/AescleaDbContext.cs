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
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }

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

            // Configure SubscriptionPlan entity
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.ToTable("SubscriptionPlans");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").HasMaxLength(50);
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Price).HasColumnName("Price").HasColumnType("decimal(18,2)");
                entity.Property(e => e.Interval).HasColumnName("Interval").IsRequired().HasMaxLength(20);
                entity.Property(e => e.FeaturesJson).HasColumnName("FeaturesJson").HasColumnType("text");
                entity.Property(e => e.MaxPatients).HasColumnName("MaxPatients");
                entity.Property(e => e.AiAnalysisLimit).HasColumnName("AiAnalysisLimit");
                entity.Property(e => e.Priority).HasColumnName("Priority");
                entity.Property(e => e.Recommended).HasColumnName("Recommended").HasDefaultValue(false);
                entity.Property(e => e.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
            });

            // Configure UserSubscription entity
            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.ToTable("UserSubscriptions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id").HasMaxLength(50);
                entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired().HasMaxLength(50);
                entity.Property(e => e.PlanId).HasColumnName("PlanId").IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).HasColumnName("Status").IsRequired().HasMaxLength(20);
                entity.Property(e => e.CurrentPeriodStart).HasColumnName("CurrentPeriodStart");
                entity.Property(e => e.CurrentPeriodEnd).HasColumnName("CurrentPeriodEnd");
                entity.Property(e => e.CancelAtPeriodEnd).HasColumnName("CancelAtPeriodEnd").HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                // Foreign key relationships
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Plan)
                      .WithMany(p => p.UserSubscriptions)
                      .HasForeignKey(e => e.PlanId)
                      .OnDelete(DeleteBehavior.Restrict);

                // Ensure one active subscription per user
                entity.HasIndex(e => new { e.UserId, e.Status })
                      .HasDatabaseName("IX_UserSubscriptions_UserId_Status");
            });
        }
    }
}
