using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.AppointmentSlot
{
    /// <summary>
    /// DTO para generar slots masivamente
    /// </summary>
    public class GenerateSlotsDto
    {
        /// <summary>
        /// ID del profesional
        /// </summary>
        [Required]
        public Guid ProfessionalId { get; set; }

        /// <summary>
        /// Fecha desde
        /// </summary>
        [Required]
        public DateOnly DateFrom { get; set; }

        /// <summary>
        /// Fecha hasta
        /// </summary>
        [Required]
        public DateOnly DateTo { get; set; }
    }
}