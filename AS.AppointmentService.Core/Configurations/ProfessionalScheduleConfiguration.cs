using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AS.AppointmentService.Core.Configurations
{
    public class ProfessionalScheduleConfiguration : IEntityTypeConfiguration<ProfessionalSchedule>
    {
        public void Configure(EntityTypeBuilder<ProfessionalSchedule> builder)
        {

            builder.ToTable("professional_schedule");

            builder.HasKey(ph => ph.Id);

           
            

                   
        }
    }
}