using AS.AppointmentService.Application.Dtos;
using AS.AppointmentService.Application.Services.Intefaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class EstadoCitaService : IEstadoCitaService
    {
        private readonly IGenericRepository<EstadoCita> _estadoCitaRepository;
        private readonly IMapper _mapper;

        public EstadoCitaService(IGenericRepository<EstadoCita> estadoCitaRepository, IMapper mapper)
        {
            _estadoCitaRepository = estadoCitaRepository;
            _mapper = mapper;
        }

        public async Task<List<EstadoCitaDto>> GetAllAsync()
        {
            try
            {
                var estados = _estadoCitaRepository.GetAllAsync();

                return _mapper.Map<List<EstadoCitaDto>>(estados);
            }
            catch{
                throw;
            }
            

        }

        public async Task<EstadoCitaDto?> GetByIdAsync(int id)
        {
            try
            {
                var estado = _estadoCitaRepository.GetByIdAsync(id);

                if (estado == null)
                    throw new TaskCanceledException("Estado de Cita no encontrado");

                return _mapper.Map<EstadoCitaDto>(estado);
            }
            catch
            {
                throw;
            }
        }
    }
}
