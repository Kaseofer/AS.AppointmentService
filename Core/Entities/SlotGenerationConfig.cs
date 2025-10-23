using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.AppointmentService.Core.Entities
{
    [Table("slot_generation_config")]
    public class SlotGenerationConfig
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("professional_id")]
        public Guid ProfessionalId { get; set; }

        [Column("advance_booking_days")]
        public int AdvanceBookingDays { get; set; } = 60;

        [Column("min_advance_hours")]
        public int MinAdvanceHours { get; set; } = 24;

        [Column("allow_same_day_booking")]
        public bool AllowSameDayBooking { get; set; } = false;

        [Column("auto_generate_slots")]
        public bool AutoGenerateSlots { get; set; } = true;

        [Column("max_appointments_per_day")]
        public int? MaxAppointmentsPerDay { get; set; }

        [Column("buffer_time_mins")]
        public int BufferTimeMins { get; set; } = 0;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}