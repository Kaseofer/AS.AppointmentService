using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.NationalHoliday
{
    public class CreateNationalHolidayDto
    {
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Máximo 100 caracteres")]
        public string Name { get; set; }

        public bool IsRecurring { get; set; } = false;

        public bool Active { get; set; } = true;
    }
}