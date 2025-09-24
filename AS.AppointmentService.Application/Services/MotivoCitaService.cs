using AS.AppointmentService.Application.Dtos;
using AS.AppointmentService.Application.Services.Intefaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    
    public class MotivoCitaService : IMotivoCitaService
    {
        private readonly IGenericRepository<MotivoCita> _motivoRepository;
        private readonly IMapper _mapper;

        public MotivoCitaService(IGenericRepository<MotivoCita> motivoRepository, IMapper mapper)
        {
            _motivoRepository = motivoRepository;
            _mapper = mapper;
        }


        public async Task<List<MotivoCitaDto>> GetAllAsync()
        {
            try
            {
                var motivos = await _motivoRepository.GetAllAsync();
                return _mapper.Map<List<MotivoCitaDto>>(motivos);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<MotivoCitaDto?> GetByIdAsync(int id)
        {
            try
            {
                var motivo = await _motivoRepository.GetByIdAsync(id);

                if (motivo == null)
                    throw new TaskCanceledException("No se encontró el motivo");

                return _mapper.Map<MotivoCitaDto>(motivo);
            }
            catch 
            {

                throw;
            }
            
        }

    }
}
