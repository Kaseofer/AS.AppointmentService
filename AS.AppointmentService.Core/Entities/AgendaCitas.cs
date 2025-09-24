using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Core.Entities
{
    public class AgendaCitas
    {
        [Key]
        public Guid Id { get; set; }
       
        public DateOnly Fecha { get; set; }

        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }

        public bool Ocupado { get; set; }

        public Guid ProfesionalId { get; set; }

        public Guid PacienteId { get; set; }
        public int MotivoCitaId { get; set; }
        public required MotivoCita MotivoCita { get; set; }

        public int EstadoCitaId { get; set; }
        public required EstadoCita EstadoCita { get; set; }

        public Guid UsuarioId { get; set; }

        public bool Vencida { get; set; }   


    }
}