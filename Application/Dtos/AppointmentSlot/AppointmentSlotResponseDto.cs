namespace AS.AppointmentService.Application.Dtos.AppointmentSlot
{
    /// <summary>
    /// DTO de respuesta para slot de turno
    /// </summary>
    public class AppointmentSlotResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProfessionalId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int DurationMins { get; set; }
        public bool IsAvailable { get; set; }
        public Guid? AppointmentId { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? BookedAt { get; set; }

        // Propiedades calculadas
        public string DisplayText => $"{Date:dd/MM/yyyy} {StartTime:HH:mm} - {EndTime:HH:mm}";
        public string StatusText => IsAvailable ? "Disponible" : "Ocupado";
    }
}