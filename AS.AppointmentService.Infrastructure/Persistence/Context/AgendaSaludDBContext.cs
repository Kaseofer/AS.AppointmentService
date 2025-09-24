using AS.AppointmentService.Core.Configurations;
using AS.AppointmentService.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AS.AppointmentService.Infrastructure.Persistence.Context
{
    public class AgendaSaludDBContext : DbContext
    {
        public AgendaSaludDBContext(DbContextOptions<AgendaSaludDBContext> options) : base(options) { }


        public DbSet<AgendaCitas> Turnos { get; set; }
        public DbSet<EstadoCita> EstadosTurno { get; set; }
        public DbSet<MotivoCita> Motivos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplicar las configuraciones de las entidades

    
            modelBuilder.ApplyConfiguration(new EstadoCitaConfiguration());
            modelBuilder.ApplyConfiguration(new MotivoCitaConfiguration());
            modelBuilder.ApplyConfiguration(new AgendaCitasConfiguration());

            // Filtro global para no traer citas vencidas
            modelBuilder.Entity<AgendaCitas>()
            .HasQueryFilter(c => !c.Vencida);
        }
    }
}

