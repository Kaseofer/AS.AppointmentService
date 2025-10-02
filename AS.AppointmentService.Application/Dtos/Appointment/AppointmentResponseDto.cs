using AS.AppointmentService.Application.Dtos.AppointmentReason;
using AS.AppointmentService.Application.Dtos.AppointmentStatus;

namespace AS.AppointmentService.Application.Dtos.Appointment
{
    public class AppointmentResponseDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsBooked { get; set; }
        public bool IsExpired { get; set; }

        // IDs
        public Guid ProfessionalId { get; set; }
        public Guid PatientId { get; set; }
        public int ReasonId { get; set; }
        public int StatusId { get; set; }
        public Guid UserId { get; set; }

        // Propiedades calculadas
        public string AppointmentDateTime => $"{Date:dd/MM/yyyy} {StartTime:HH:mm}";

        // Navegaciones
        public AppointmentReasonResponseDto? Reason { get; set; }
        public AppointmentStatusResponseDto? Status { get; set; }
    }
}