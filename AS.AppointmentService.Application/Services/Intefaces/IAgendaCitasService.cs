using AS.AppointmentService.Application.Dtos;
using AS.AppointmentService.Application.Dtos.Filtros;

namespace AS.AppointmentService.Application.Services.Intefaces
{
    public interface IAgendaCitasService
    {

        Task<AgendaCitasDto> CreateAsync(AgendaCitasDto agendaCitasDto);

        Task<List<AgendaCitasDto>> GetAllAsync();
        
        Task<AgendaCitasDto?> GetByIdAsync(int id);

        Task<List<AgendaCitasDto>> FindAsync(AgendaCitaFiltroDto filtro);

        Task<bool> UpdateAsync(AgendaCitasDto agendaCitasDto);

        Task<bool> DeleteAsync(int id);
    }
}
