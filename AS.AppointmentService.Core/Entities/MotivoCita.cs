using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Core.Entities
{
    public class MotivoCita
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

    }
}