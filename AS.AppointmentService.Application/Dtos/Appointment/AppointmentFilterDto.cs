namespace AS.AppointmentService.Application.Dtos.Appointment
{
    public class AppointmentFilterDto
    {
        // Filtros
        public Guid? ProfessionalId { get; set; }
        public Guid? PatientId { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
        public int? StatusId { get; set; }
        public bool? IsBooked { get; set; }
        public bool? IsExpired { get; set; }

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Ordenamiento
        public string? OrderBy { get; set; } = "Date";
        public string? OrderDirection { get; set; } = "asc";
    }
}