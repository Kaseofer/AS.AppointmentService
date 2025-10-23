using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AS.AppointmentService.Core.Entities
{
    [Table("appointment_reason")]
    public class AppointmentReason
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Column("description")]
        [StringLength(500)]
        public string? Description { get; set; }
    }
}