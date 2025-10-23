using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.ProfessionalNonWorkingDay
{
    public class CreateProfessionalNonWorkingDayDto
    {
        [Required(ErrorMessage = "El profesional es obligatorio")]
        public Guid ProfessionalId { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateOnly Date { get; set; }

        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string? Reason { get; set; }

        public bool AllDay { get; set; } = true;

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }
    }
}