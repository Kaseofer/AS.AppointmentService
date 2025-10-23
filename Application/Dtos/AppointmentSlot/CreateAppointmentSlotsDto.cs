using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.AppointmentSlot
{
    /// <summary>
    /// DTO para crear un nuevo slot de turno
    /// </summary>
    public class CreateAppointmentSlotDto
    {
        /// <summary>
        /// ID del profesional
        /// </summary>
        [Required(ErrorMessage = "El profesional es obligatorio")]
        public Guid ProfessionalId { get; set; }

        /// <summary>
        /// Fecha del slot
        /// </summary>
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateOnly Date { get; set; }

        /// <summary>
        /// Hora de inicio
        /// </summary>
        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeOnly StartTime { get; set; }

        /// <summary>
        /// Hora de fin
        /// </summary>
        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        public TimeOnly EndTime { get; set; }

        /// <summary>
        /// Duración en minutos
        /// </summary>
        [Required]
        [Range(5, 240, ErrorMessage = "La duración debe estar entre 5 y 240 minutos")]
        public int DurationMins { get; set; }
    }
}