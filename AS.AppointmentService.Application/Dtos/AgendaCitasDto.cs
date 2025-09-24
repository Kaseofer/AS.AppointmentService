using AS.AppointmentService.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos
{
    public class AgendaCitasDto
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
        public MotivoCitaDto MotivoCita { get; set; }

        public int EstadoCitaId { get; set; }
        public EstadoCitaDto EstadoCita { get; set; }

        public Guid UsuarioId { get; set; }

        public bool Vencida { get; set; }
    }
}