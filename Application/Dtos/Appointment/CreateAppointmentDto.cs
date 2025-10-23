using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.Appointment
{
    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "El profesional es obligatorio")]
        public Guid ProfessionalId { get; set; }

        [Required(ErrorMessage = "El paciente es obligatorio")]
        public Guid PatientId { get; set; }

        [Required(ErrorMessage = "El motivo de la cita es obligatorio")]
        public int ReasonId { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        public Guid UserId { get; set; }
    }
}