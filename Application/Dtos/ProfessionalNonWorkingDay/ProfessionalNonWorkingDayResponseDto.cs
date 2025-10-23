namespace AS.AppointmentService.Application.Dtos.ProfessionalNonWorkingDay
{
    public class ProfessionalNonWorkingDayResponseDto
    {
        public int Id { get; set; }
        public Guid ProfessionalId { get; set; }
        public DateOnly Date { get; set; }
        public string? Reason { get; set; }
        public bool AllDay { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }

        // Propiedades calculadas
        public string DisplayText => AllDay
            ? $"{Date:dd/MM/yyyy} - Todo el día"
            : $"{Date:dd/MM/yyyy} de {StartTime:HH:mm} a {EndTime:HH:mm}";
    }
}