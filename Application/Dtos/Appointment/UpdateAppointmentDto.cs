using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos.Appointment
{
    public class UpdateAppointmentDto
    {
        public DateOnly? Date { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public Guid? ProfessionalId { get; set; }

        public Guid? PatientId { get; set; }

        public int? ReasonId { get; set; }

        public int? StatusId { get; set; }
    }
}