using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.ProfessionalNonWorkingDay
{
    public class UpdateProfessionalNonWorkingDayDto
    {
        public DateOnly? Date { get; set; }

        [StringLength(200, ErrorMessage = "Máximo 200 caracteres")]
        public string? Reason { get; set; }

        public bool? AllDay { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }
    }
}