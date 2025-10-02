using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.Professional
{
    public class UpdateProfessionalScheduleDto
    {
        [Range(0, 6, ErrorMessage = "El día debe estar entre 0 (Domingo) y 6 (Sábado)")]
        public int? DayOfWeek { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Range(10, 60, ErrorMessage = "La duración debe estar entre 10 y 60 minutos")]
        public int? AppointmentDurationMins { get; set; }
    }
}