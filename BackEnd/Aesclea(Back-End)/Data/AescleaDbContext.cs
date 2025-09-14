// Copyright (c) 2025 Alexander Sinapov | Simeon Petkov

// All rights reserved.
// This code is proprietary and confidential.  
// Unauthorized copying, modification, distribution, or use is strictly prohibited.

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
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        public DbSet<SubscriptionUsage> SubscriptionUsages { get; set; }
        public DbSet<AnalysisRecord> AnalysisRecords { get; set; }

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
                entity.Property(e => e.IsEmailVerified).HasColumnName("IsEmailVerified").HasDefaultValue(false);
                entity.Property(e => e.EmailVerificationToken).HasColumnName("EmailVerificationToken");
                entity.Property(e => e.EmailVerificationTokenExpires).HasColumnName("EmailVerificationTokenExpires");
                entity.Property(e => e.PasswordResetToken).HasColumnName("PasswordResetToken");
                entity.Property(e => e.PasswordResetTokenExpiry).HasColumnName("PasswordResetTokenExpires");
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
                entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
                entity.Property(e => e.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasColumnName("Email").HasMaxLength(200);
                entity.Property(e => e.Phone).HasColumnName("Phone").HasMaxLength(20);
                entity.Property(e => e.DateOfBirth).HasColumnName("DateOfBirth");
                entity.Property(e => e.Gender).HasColumnName("Gender").HasMaxLength(20);
                entity.Property(e => e.MedicalHistory).HasColumnName("MedicalHistory");
                entity.Property(e => e.Department).HasColumnName("Department").HasMaxLength(100);
                entity.Property(e => e.Status).HasColumnName("Status").IsRequired().HasMaxLength(50).HasDefaultValue("active");
                entity.Property(e => e.LastVisit).HasColumnName("LastVisit");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                // Configure relationship
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Create indexes for performance
                entity.HasIndex(e => e.Email);
                entity.HasIndex(e => e.Department);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.LastName);
            });

            // Configure SubscriptionPlan entity
            modelBuilder.Entity<SubscriptionPlan>(entity =>
            {
                entity.ToTable("SubscriptionPlans");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name").IsRequired();
                entity.Property(e => e.Price).HasColumnName("Price").HasColumnType("decimal(18,2)");
                entity.Property(e => e.Interval).HasColumnName("Interval").IsRequired();
                entity.Property(e => e.Features).HasColumnName("Features")
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                entity.Property(e => e.MaxPatients).HasColumnName("MaxPatients");
                entity.Property(e => e.AiAnalysisLimit).HasColumnName("AiAnalysisLimit");
                entity.Property(e => e.Priority).HasColumnName("Priority");
                entity.Property(e => e.Recommended).HasColumnName("Recommended");
                entity.Property(e => e.IsActive).HasColumnName("IsActive");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");
            });

            // Configure UserSubscription entity
            modelBuilder.Entity<UserSubscription>(entity =>
            {
                entity.ToTable("UserSubscriptions");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
                entity.Property(e => e.PlanId).HasColumnName("PlanId").IsRequired();
                entity.Property(e => e.Status).HasColumnName("Status").IsRequired();
                entity.Property(e => e.CurrentPeriodStart).HasColumnName("CurrentPeriodStart");
                entity.Property(e => e.CurrentPeriodEnd).HasColumnName("CurrentPeriodEnd");
                entity.Property(e => e.CancelAtPeriodEnd).HasColumnName("CancelAtPeriodEnd");
                entity.Property(e => e.CanceledAt).HasColumnName("CanceledAt");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                // Configure relationships
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Plan)
                    .WithMany()
                    .HasForeignKey(e => e.PlanId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure SubscriptionUsage entity
            modelBuilder.Entity<SubscriptionUsage>(entity =>
            {
                entity.ToTable("SubscriptionUsages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.UserId).HasColumnName("UserId").IsRequired();
                entity.Property(e => e.Feature).HasColumnName("Feature").IsRequired();
                entity.Property(e => e.UsageCount).HasColumnName("UsageCount");
                entity.Property(e => e.ResetDate).HasColumnName("ResetDate");
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                // Configure relationship
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Create unique index on UserId + Feature
                entity.HasIndex(e => new { e.UserId, e.Feature }).IsUnique();
            });

            // Configure AnalysisRecord entity
            modelBuilder.Entity<AnalysisRecord>(entity =>
            {
                entity.ToTable("AnalysisRecords");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.PatientId).HasColumnName("PatientId").IsRequired();
                entity.Property(e => e.PatientName).HasColumnName("PatientName");
                entity.Property(e => e.AnalysisType).HasColumnName("AnalysisType").IsRequired();
                entity.Property(e => e.Status).HasColumnName("Status").IsRequired();
                entity.Property(e => e.Department).HasColumnName("Department");
                entity.Property(e => e.Notes).HasColumnName("Notes");
                entity.Property(e => e.Data).HasColumnName("Data");
                entity.Property(e => e.Results).HasColumnName("Results");
                entity.Property(e => e.Confidence).HasColumnName("Confidence");
                entity.Property(e => e.Recommendations).HasColumnName("Recommendations");
                entity.Property(e => e.CreatedBy).HasColumnName("CreatedBy").IsRequired();
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt");
                entity.Property(e => e.CompletedAt).HasColumnName("CompletedAt");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                // Configure relationships
                entity.HasOne(e => e.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedBy)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasOne(e => e.Patient)
                    .WithMany()
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure Appointment entity
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.ToTable("Appointments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.PatientId).HasColumnName("PatientId").IsRequired();
                entity.Property(e => e.PatientName).HasColumnName("PatientName").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Department).HasColumnName("Department").IsRequired().HasMaxLength(100);
                entity.Property(e => e.Doctor).HasColumnName("Doctor").IsRequired().HasMaxLength(200);
                entity.Property(e => e.AppointmentType).HasColumnName("AppointmentType").IsRequired().HasMaxLength(100);
                entity.Property(e => e.DateTime).HasColumnName("DateTime").IsRequired();
                entity.Property(e => e.Duration).HasColumnName("Duration").IsRequired();
                entity.Property(e => e.Status).HasColumnName("Status").IsRequired().HasMaxLength(50).HasDefaultValue("scheduled");
                entity.Property(e => e.Priority).HasColumnName("Priority").IsRequired().HasMaxLength(50).HasDefaultValue("normal");
                entity.Property(e => e.Reason).HasColumnName("Reason").HasMaxLength(1000);
                entity.Property(e => e.Notes).HasColumnName("Notes").HasMaxLength(2000);
                entity.Property(e => e.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("NOW()");
                entity.Property(e => e.UpdatedAt).HasColumnName("UpdatedAt");

                // Configure relationships
                entity.HasOne(e => e.Patient)
                    .WithMany()
                    .HasForeignKey(e => e.PatientId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Create indexes
                entity.HasIndex(e => e.PatientId);
                entity.HasIndex(e => e.DateTime);
                entity.HasIndex(e => e.Department);
                entity.HasIndex(e => e.Status);
            });
        }
    }
}
