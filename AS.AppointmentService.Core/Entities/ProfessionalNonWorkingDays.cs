using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.AppointmentService.Core.Entities
{
    [Table("professional_non_working_days")]
    public class ProfessionalNonWorkingDay
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("professional_id")]
        public Guid ProfessionalId { get; set; }

        [Required]
        [Column("date")]
        public DateOnly Date { get; set; }

        [Column("reason")]
        [StringLength(200)]
        public string? Reason { get; set; }

        [Column("all_day")]
        public bool AllDay { get; set; } = true;

        [Column("start_time")]
        public TimeOnly? StartTime { get; set; }

        [Column("end_time")]
        public TimeOnly? EndTime { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }


    }
}