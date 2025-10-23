using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.AppointmentService.Core.Configurations
{
    public class AppointmentStatusConfiguration : IEntityTypeConfiguration<AppointmentStatus>
    {
        public void Configure(EntityTypeBuilder<AppointmentStatus> builder)
        {
            builder.ToTable("appointment_status");

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
                   .HasDatabaseName("idx_appointment_status_name");
        }
    }
}