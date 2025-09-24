using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.AppointmentService.Core.Configurations
{
    public class AgendaCitasConfiguration : IEntityTypeConfiguration<AgendaCitas>
    {
        public void Configure(EntityTypeBuilder<AgendaCitas> builder)
        {
            builder.ToTable("agenda_citas", "agendasalud");

            builder.HasKey(a => a.Id);


            builder.HasOne(t => t.MotivoCita)
                   .WithMany()
                   .HasForeignKey(t => t.MotivoCitaId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.EstadoCita)
                   .WithMany()
                   .HasForeignKey(t => t.EstadoCitaId)
                   .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}