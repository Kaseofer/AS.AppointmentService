using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.SlotGenerationConfig
{
    /// <summary>
    /// DTO para crear configuración de generación de slots
    /// </summary>
    public class CreateSlotGenerationConfigDto
    {
        /// <summary>
        /// ID del profesional
        /// </summary>
        [Required(ErrorMessage = "El profesional es obligatorio")]
        public Guid ProfessionalId { get; set; }

        /// <summary>
        /// Días de anticipación para reservar (1-365)
        /// </summary>
        [Range(1, 365, ErrorMessage = "Debe estar entre 1 y 365 días")]
        public int AdvanceBookingDays { get; set; } = 60;

        /// <summary>
        /// Horas mínimas de anticipación (0-168)
        /// </summary>
        [Range(0, 168, ErrorMessage = "Debe estar entre 0 y 168 horas")]
        public int MinAdvanceHours { get; set; } = 24;

        /// <summary>
        /// Permite reservar el mismo día
        /// </summary>
        public bool AllowSameDayBooking { get; set; } = false;

        /// <summary>
        /// Genera slots automáticamente
        /// </summary>
        public bool AutoGenerateSlots { get; set; } = true;

        /// <summary>
        /// Máximo de turnos por día (null = sin límite)
        /// </summary>
        [Range(1, 100, ErrorMessage = "Debe estar entre 1 y 100")]
        public int? MaxAppointmentsPerDay { get; set; }

        /// <summary>
        /// Tiempo de descanso entre turnos en minutos (0-60)
        /// </summary>
        [Range(0, 60, ErrorMessage = "Debe estar entre 0 y 60 minutos")]
        public int BufferTimeMins { get; set; } = 0;
    }
}