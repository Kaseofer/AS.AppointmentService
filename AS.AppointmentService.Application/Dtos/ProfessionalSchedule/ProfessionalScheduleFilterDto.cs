namespace AS.AppointmentService.Application.Dtos.Professional
{
    public class ProfessionalScheduleFilterDto
    {
        public Guid? ProfessionalId { get; set; }
        public int? DayOfWeek { get; set; }

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Ordenamiento
        public string? OrderBy { get; set; } = "DayOfWeek";
        public string? OrderDirection { get; set; } = "asc";
    }
}