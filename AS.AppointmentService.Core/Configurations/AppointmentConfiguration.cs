using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.AppointmentService.Core.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("appointment", "agendasalud");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .HasColumnName("id");

            builder.Property(a => a.Date)
                   .HasColumnName("date")
                   .IsRequired();

            builder.Property(a => a.StartTime)
                   .HasColumnName("start_time")
                   .IsRequired();

            builder.Property(a => a.EndTime)
                   .HasColumnName("end_time")
                   .IsRequired();

            builder.Property(a => a.IsBooked)
                   .HasColumnName("is_booked")
                   .HasDefaultValue(false);

            builder.Property(a => a.ProfessionalId)
                   .HasColumnName("professional_id")
                   .IsRequired();

            builder.Property(a => a.PatientId)
                   .HasColumnName("patient_id")
                   .IsRequired();

            builder.Property(a => a.ReasonId)
                   .HasColumnName("reason_id")
                   .IsRequired();

            builder.Property(a => a.StatusId)
                   .HasColumnName("status_id")
                   .IsRequired();

            builder.Property(a => a.UserId)
                   .HasColumnName("user_id")
                   .IsRequired();

            builder.Property(a => a.IsExpired)
                   .HasColumnName("is_expired")
                   .HasDefaultValue(false);

            // Relaciones
            builder.HasOne(a => a.Reason)
                   .WithMany()
                   .HasForeignKey(a => a.ReasonId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Status)
                   .WithMany()
                   .HasForeignKey(a => a.StatusId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Índices
            builder.HasIndex(a => a.ProfessionalId)
                   .HasDatabaseName("idx_appointment_professional_id");

            builder.HasIndex(a => a.PatientId)
                   .HasDatabaseName("idx_appointment_patient_id");

            builder.HasIndex(a => a.Date)
                   .HasDatabaseName("idx_appointment_date");

            builder.HasIndex(a => a.StatusId)
                   .HasDatabaseName("idx_appointment_status_id");
        }
    }
}