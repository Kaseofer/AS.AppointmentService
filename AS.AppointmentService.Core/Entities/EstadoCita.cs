using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Core.Entities
{
    public class EstadoCita
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ColorHex { get; set; }
    }
}