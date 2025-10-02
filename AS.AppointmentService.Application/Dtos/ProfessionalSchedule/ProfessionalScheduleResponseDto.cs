namespace AS.AppointmentService.Application.Dtos.Professional
{
    public class ProfessionalScheduleResponseDto
    {
        public int Id { get; set; }
        public Guid ProfessionalId { get; set; }
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int AppointmentDurationMins { get; set; }

        // Propiedades calculadas
        public string DayOfWeekText => DayOfWeek switch
        {
            0 => "Domingo",
            1 => "Lunes",
            2 => "Martes",
            3 => "Miércoles",
            4 => "Jueves",
            5 => "Viernes",
            6 => "Sábado",
            _ => "Desconocido"
        };

        public string ScheduleText => $"{DayOfWeekText} de {StartTime:hh\\:mm} a {EndTime:hh\\:mm}";

        
    }
}