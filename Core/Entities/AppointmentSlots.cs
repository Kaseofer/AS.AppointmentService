using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.AppointmentService.Core.Entities
{
    [Table("appointment_slots")]
    public class AppointmentSlots
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("professional_id")]
        public Guid ProfessionalId { get; set; }

        [Required]
        [Column("date")]
        public DateOnly Date { get; set; }

        [Required]
        [Column("start_time")]
        public TimeOnly StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public TimeOnly EndTime { get; set; }

        [Required]
        [Column("duration_mins")]
        public int DurationMins { get; set; }

        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;

        [Column("appointment_id")]
        public Guid? AppointmentId { get; set; }

        [Column("generated_at")]
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        [Column("booked_at")]
        public DateTime? BookedAt { get; set; }

        // Navegación
        [ForeignKey("AppointmentId")]
        public virtual Appointment? Appointment { get; set; }
    }
}