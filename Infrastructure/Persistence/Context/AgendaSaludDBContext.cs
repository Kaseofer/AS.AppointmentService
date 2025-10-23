using AS.AppointmentService.Core.Configurations;
using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AS.AppointmentService.Infrastructure.Persistence.Context
{
    public class AgendaSaludDBContext : DbContext
    {
        public AgendaSaludDBContext(DbContextOptions<AgendaSaludDBContext> options) : base(options) { }


        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AppointmentStatus> AppointmentStatus { get; set; }
        public DbSet<AppointmentReason> AppointmentReasons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplicar las configuraciones de las entidades

    
            modelBuilder.ApplyConfiguration(new AppointmentStatusConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentReasonConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());

            // Filtro global para no traer citas vencidas
            modelBuilder.Entity<Appointment>()
            .HasQueryFilter(c => !c.IsExpired);
        }
    }
}

