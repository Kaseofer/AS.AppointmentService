namespace AS.AppointmentService.Application.Dtos.AppointmentSlot
{
    /// <summary>
    /// DTO para filtrar slots de turnos
    /// </summary>
    public class AppointmentSlotFilterDto
    {
        /// <summary>
        /// Filtrar por profesional
        /// </summary>
        public Guid? ProfessionalId { get; set; }

        /// <summary>
        /// Fecha desde
        /// </summary>
        public DateOnly? DateFrom { get; set; }

        /// <summary>
        /// Fecha hasta
        /// </summary>
        public DateOnly? DateTo { get; set; }

        /// <summary>
        /// Filtrar solo disponibles o solo ocupados
        /// </summary>
        public bool? IsAvailable { get; set; }

        /// <summary>
        /// Hora desde
        /// </summary>
        public TimeOnly? TimeFrom { get; set; }

        /// <summary>
        /// Hora hasta
        /// </summary>
        public TimeOnly? TimeTo { get; set; }

        // Paginación
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;

        // Ordenamiento
        public string? OrderBy { get; set; } = "Date";
        public string? OrderDirection { get; set; } = "asc";
    }
}