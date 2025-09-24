using AS.AppointmentService.Application.Dtos;

namespace AS.AppointmentService.Application.Services.Intefaces
{
    public interface IMotivoCitaService
    {

        Task<List<MotivoCitaDto>> GetAllAsync();

        Task<MotivoCitaDto?> GetByIdAsync(int id);
    }
}
