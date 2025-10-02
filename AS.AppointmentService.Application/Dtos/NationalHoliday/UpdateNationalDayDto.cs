using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.NationalHoliday
{
    public class UpdateNationalHolidayDto
    {
        public DateOnly? Date { get; set; }

        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string? Name { get; set; }

        public bool? IsRecurring { get; set; }

        public bool? Active { get; set; }
    }
}