using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.AppointmentService.Core.Configurations
{
    public class EstadoCitaConfiguration : IEntityTypeConfiguration<EstadoCita>
    {
        public void Configure(EntityTypeBuilder<EstadoCita> builder)
        {
            builder.ToTable("estado_cita", "agendasalud");

            builder.HasKey(e => e.Id);
            


        }
    }
}