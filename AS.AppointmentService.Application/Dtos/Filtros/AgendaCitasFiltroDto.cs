namespace AS.AppointmentService.Application.Dtos.Filtros
{
    public class AgendaCitaFiltroDto
    {
        public DateOnly? FechaDesde { get; set; }
        public DateOnly? FechaHasta { get; set; }

        public int? ProfesionalId { get; set; }
        public int? PacienteId { get; set; }

        public int? EstadoCitaId { get; set; }
        public int? MotivoCitaId { get; set; }

        public bool? Ocupado { get; set; }
    }
}
