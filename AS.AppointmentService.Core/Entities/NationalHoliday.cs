using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.AppointmentService.Core.Entities
{
    [Table("national_holidays")]
    public class NationalHoliday
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("date")]
        public DateOnly Date { get; set; }

        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Column("is_recurring")]
        public bool IsRecurring { get; set; } = false;

        [Column("active")]
        public bool Active { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}