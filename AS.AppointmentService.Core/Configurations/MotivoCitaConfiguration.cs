using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.AppointmentService.Core.Configurations
{
    public class MotivoCitaConfiguration : IEntityTypeConfiguration<MotivoCita>
    {
        public void Configure(EntityTypeBuilder<MotivoCita> builder)
        {
            builder.ToTable("motivo_cita", "agendasalud");

            builder.HasKey(m => m.Id);


        }
    }
}