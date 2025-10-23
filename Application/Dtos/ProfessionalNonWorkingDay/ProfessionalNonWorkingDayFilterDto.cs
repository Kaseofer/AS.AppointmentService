namespace AS.AppointmentService.Application.Dtos.ProfessionalNonWorkingDay
{
    public class ProfessionalNonWorkingDayFilterDto
    {
        public Guid? ProfessionalId { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
        public bool? AllDay { get; set; }

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Ordenamiento
        public string? OrderBy { get; set; } = "Date";
        public string? OrderDirection { get; set; } = "asc";
    }
}