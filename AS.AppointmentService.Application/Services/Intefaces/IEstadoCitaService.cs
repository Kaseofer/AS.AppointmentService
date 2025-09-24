using AS.AppointmentService.Application.Dtos;

namespace AS.AppointmentService.Application.Services.Intefaces
{
    public interface IEstadoCitaService
    {
        Task<List<EstadoCitaDto>> GetAllAsync();
        Task<EstadoCitaDto?> GetByIdAsync(int id);
    }
}
