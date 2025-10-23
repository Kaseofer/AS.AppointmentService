namespace AS.AppointmentService.Application.Dtos.NationalHoliday
{
    public class NationalHolidayResponseDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public string Name { get; set; }
        public bool IsRecurring { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propiedades calculadas
        public string DisplayText => $"{Date:dd/MM/yyyy} - {Name}";
        public string TypeText => IsRecurring ? "Anual" : "Único";
    }
}