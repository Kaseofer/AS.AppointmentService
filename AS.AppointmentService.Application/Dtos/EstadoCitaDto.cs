using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos
{
    public class EstadoCitaDto
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ColorHex { get; set; }
    }
}