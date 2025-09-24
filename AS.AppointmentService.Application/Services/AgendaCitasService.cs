using AS.AppointmentService.Application.Dtos;
using AS.AppointmentService.Application.Dtos.Filtros;
using AS.AppointmentService.Application.Services.Intefaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class AgendaCitasService: IAgendaCitasService
    {
        private readonly IGenericRepository<AgendaCitas> _agendaCitasRepository;
        private readonly IMapper _mapper;

        public AgendaCitasService(IGenericRepository<AgendaCitas> agendaCitasRepository, IMapper mapper)
        {
            _agendaCitasRepository = agendaCitasRepository;
            _mapper = mapper;
        }

        public async Task<AgendaCitasDto> CreateAsync(AgendaCitasDto agendaCitasDto)
        {
            try
            {
                var nuevaAgendaCitas = await _agendaCitasRepository.AddAsync(_mapper.Map<AgendaCitas>(agendaCitasDto));
            
                if(nuevaAgendaCitas.Id == Guid.Empty)
                    throw new TaskCanceledException("No se pudo crear la agenda de citas");

                return _mapper.Map<AgendaCitasDto>(nuevaAgendaCitas);
            }
            catch 
            {

                throw;
            }
            

        }

        public async Task<bool> UpdateAsync(AgendaCitasDto agendaCitasDto)
        {
            try
            {
                var agendaCitasExistente = await _agendaCitasRepository.GetByIdAsync(agendaCitasDto.Id);
                
                if (agendaCitasExistente == null)
                    throw new TaskCanceledException("No se encontró la agenda de citas");
               
                var isSuccess = await _agendaCitasRepository.UpdateAsync(_mapper.Map<AgendaCitas>(agendaCitasDto));
                
                if (isSuccess == false)
                    throw new TaskCanceledException("No se pudo actualizar la agenda de citas");

                return isSuccess;
            }
            catch 
            {
                throw ;
            }

        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var agendaCitasALiberar = await _agendaCitasRepository.GetByIdAsync(id);
                
                if (agendaCitasALiberar == null)
                    throw new TaskCanceledException("No se encontró la agenda de citas");
           
                var isSuccess = await _agendaCitasRepository.RemoveAsync(agendaCitasALiberar);
                
                if (isSuccess == false)
                    throw new TaskCanceledException("No se pudo eliminar la agenda de citas");
               
                return isSuccess;
            }
            catch 
            {
                throw ;
            }
        }


        public async Task<AgendaCitasDto?> GetByIdAsync(int id)
        {
            try
            {
                var agendaCitas = await _agendaCitasRepository.GetByIdAsync(id);

                if (agendaCitas == null)
                    throw new TaskCanceledException("No se encontró la agenda de citas");
            
                return _mapper.Map<AgendaCitasDto>(agendaCitas);
            }
            catch 
            {

                throw;
            }
            
        }

        public async Task<List<AgendaCitasDto>> GetAllAsync()
        {
            try
            {
                var agendaCitas = await _agendaCitasRepository.GetAllAsync();

                return _mapper.Map<List<AgendaCitasDto>>(agendaCitas);
            }
            catch 
            {
                throw;
            }
            
        }

        public async Task<List<AgendaCitasDto>> FindAsync(AgendaCitaFiltroDto filtro)
        {
            try
            {
                var citas = await _agendaCitasRepository.FindAsync(c =>
                    (!filtro.FechaDesde.HasValue || c.Fecha >= filtro.FechaDesde.Value) &&
                    (!filtro.FechaHasta.HasValue || c.Fecha <= filtro.FechaHasta.Value) &&
                    (!filtro.ProfesionalId.HasValue || c.ProfesionalId == filtro.ProfesionalId.Value) &&
                    (!filtro.PacienteId.HasValue || c.PacienteId == filtro.PacienteId.Value) &&
                    (!filtro.EstadoCitaId.HasValue || c.EstadoCitaId == filtro.EstadoCitaId.Value) &&
                    (!filtro.MotivoCitaId.HasValue || c.MotivoCitaId == filtro.MotivoCitaId.Value) &&
                    (!filtro.Ocupado.HasValue || c.Ocupado == filtro.Ocupado.Value)
                );

                return _mapper.Map<List<AgendaCitasDto>>(citas);
            }
            catch 
            {

                throw;
            }
            
        }

    }
}
