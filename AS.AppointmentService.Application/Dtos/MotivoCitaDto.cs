using System.ComponentModel.DataAnnotations;

namespace AS.AppointmentService.Application.Dtos
{
    public class MotivoCitaDto
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
    }
}