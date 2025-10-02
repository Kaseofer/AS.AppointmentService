using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.AppointmentService.Core.Entities
{
    [Table("appointment", Schema = "agendasalud")]
    public class Appointment
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("date")]
        public DateOnly Date { get; set; }

        [Required]
        [Column("start_time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public TimeOnly EndTime { get; set; }

        [Column("is_booked")]
        public bool IsBooked { get; set; } = false;

        [Required]
        [Column("professional_id")]
        public Guid ProfessionalId { get; set; }

        [Required]
        [Column("patient_id")]
        public Guid PatientId { get; set; }

        [Required]
        [Column("reason_id")]
        public int ReasonId { get; set; }

        [Required]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("is_expired")]
        public bool IsExpired { get; set; } = false;

        // Navegaciones
        [ForeignKey("ReasonId")]
        public virtual AppointmentReason Reason { get; set; }

        [ForeignKey("StatusId")]
        public virtual AppointmentStatus Status { get; set; }
    }
}