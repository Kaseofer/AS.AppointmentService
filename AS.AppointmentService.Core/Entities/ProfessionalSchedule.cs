using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.AppointmentService.Core.Entities
{
    [Table("professional_schedule")]
    public class ProfessionalSchedule
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("professional_id")]
        public Guid ProfessionalId { get; set; }

        [Required]
        [Column("day_of_week")]
        [Range(0, 6)]
        public int DayOfWeek { get; set; }

        [Required]
        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Column("appointment_duration_mins")]
        public int AppointmentDurationMins { get; set; }
        public bool Active { get; set; }
    }
}