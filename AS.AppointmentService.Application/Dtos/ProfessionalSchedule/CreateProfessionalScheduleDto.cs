using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.Professional
{
    public class CreateProfessionalScheduleDto
    {
        [Required(ErrorMessage = "El profesional es obligatorio")]
        public Guid ProfessionalId { get; set; }

        [Required(ErrorMessage = "El día de la semana es obligatorio")]
        [Range(0, 6, ErrorMessage = "El día debe estar entre 0 (Domingo) y 6 (Sábado)")]
        public int DayOfWeek { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "La duración del turno es obligatoria")]
        [Range(10, 60, ErrorMessage = "La duración debe estar entre 10 y 60 minutos")]
        public int AppointmentDurationMins { get; set; }
    }
}