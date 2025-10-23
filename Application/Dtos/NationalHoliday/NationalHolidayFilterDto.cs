namespace AS.AppointmentService.Application.Dtos.NationalHoliday
{
    public class NationalHolidayFilterDto
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
        public bool? IsRecurring { get; set; }
        public bool? Active { get; set; }

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        // Ordenamiento
        public string? OrderBy { get; set; } = "Date";
        public string? OrderDirection { get; set; } = "asc";
    }
}