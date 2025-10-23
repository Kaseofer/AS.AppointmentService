using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.AppointmentService.Core.Configurations
{
    public class AppointmentReasonConfiguration : IEntityTypeConfiguration<AppointmentReason>
    {
        public void Configure(EntityTypeBuilder<AppointmentReason> builder)
        {
            builder.ToTable("appointment_reason");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .HasColumnName("id");

            builder.Property(e => e.Name)
                   .HasColumnName("name")
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.Description)
                   .HasColumnName("description")
                   .HasMaxLength(500);

            // Índice único en el nombre
            builder.HasIndex(e => e.Name)
                   .IsUnique()
                   .HasDatabaseName("idx_appointment_reason_name");
        }
    }
}