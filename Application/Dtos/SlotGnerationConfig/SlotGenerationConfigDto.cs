namespace AS.AppointmentService.Application.Dtos.SlotGenerationConfig
{
    /// <summary>
    /// DTO de respuesta para configuración de generación de slots
    /// </summary>
    public class SlotGenerationConfigResponseDto
    {
        public int Id { get; set; }
        public Guid ProfessionalId { get; set; }
        public int AdvanceBookingDays { get; set; }
        public int MinAdvanceHours { get; set; }
        public bool AllowSameDayBooking { get; set; }
        public bool AutoGenerateSlots { get; set; }
        public int? MaxAppointmentsPerDay { get; set; }
        public int BufferTimeMins { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Propiedades calculadas
        public string AdvanceBookingText => $"{AdvanceBookingDays} días";
        public string MinAdvanceText => $"{MinAdvanceHours} horas";
        public string BufferTimeText => BufferTimeMins > 0 ? $"{BufferTimeMins} minutos" : "Sin descanso";
    }
}