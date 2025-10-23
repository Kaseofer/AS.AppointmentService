using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.Professional;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class ProfessionalScheduleService : IProfessionalScheduleService
    {
        private readonly IGenericRepository<ProfessionalSchedule> _scheduleRepository;
        private readonly IMapper _mapper;

        public ProfessionalScheduleService(
            IGenericRepository<ProfessionalSchedule> scheduleRepository,
            IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
        }

        public async Task<ProfessionalScheduleResponseDto?> GetByIdAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);

            if (schedule == null)
                throw new KeyNotFoundException("No se encontró el horario");

            return _mapper.Map<ProfessionalScheduleResponseDto>(schedule);
        }

        public async Task<List<ProfessionalScheduleResponseDto>> GetByProfessionalIdAsync(Guid professionalId)
        {
            var schedules = await _scheduleRepository.FindAsync(h => h.ProfessionalId == professionalId);
            return _mapper.Map<List<ProfessionalScheduleResponseDto>>(
                schedules.OrderBy(h => h.DayOfWeek).ThenBy(h => h.StartTime).ToList()
            );
        }

        public async Task<List<ProfessionalScheduleResponseDto>> GetAllAsync()
        {
            var schedules = await _scheduleRepository.GetAllAsync();
            return _mapper.Map<List<ProfessionalScheduleResponseDto>>(schedules);
        }

        public async Task<ProfessionalScheduleResponseDto> CreateAsync(CreateProfessionalScheduleDto dto)
        {

            // Validar que EndTime > StartTime
            if (dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("La hora de fin debe ser mayor a la hora de inicio");

            // Validar que no haya solapamiento con horarios existentes
            var existingSchedules = await _scheduleRepository.FindAsync(h =>
                h.ProfessionalId == dto.ProfessionalId &&
                h.DayOfWeek == dto.DayOfWeek);

            foreach (var existing in existingSchedules)
            {
                if ((dto.StartTime >= existing.StartTime && dto.StartTime < existing.EndTime) ||
                    (dto.EndTime > existing.StartTime && dto.EndTime <= existing.EndTime) ||
                    (dto.StartTime <= existing.StartTime && dto.EndTime >= existing.EndTime))
                {
                    throw new InvalidOperationException(
                        $"El horario se solapa con un horario existente: {existing.StartTime:hh\\:mm} - {existing.EndTime:hh\\:mm}"
                    );
                }
            }

            var schedule = _mapper.Map<ProfessionalSchedule>(dto);
            var createdSchedule = await _scheduleRepository.AddAsync(schedule);

            if (createdSchedule.Id == 0)
                throw new InvalidOperationException("No se pudo crear el horario");

            return _mapper.Map<ProfessionalScheduleResponseDto>(createdSchedule);
        }

        public async Task<ProfessionalScheduleResponseDto> UpdateAsync(int id, UpdateProfessionalScheduleDto dto)
        {
            var existingSchedule = await _scheduleRepository.GetByIdAsync(id);

            if (existingSchedule == null)
                throw new KeyNotFoundException("No se encontró el horario");

            // Mapear cambios
            _mapper.Map(dto, existingSchedule);

            // Validar EndTime > StartTime
            if (existingSchedule.EndTime <= existingSchedule.StartTime)
                throw new InvalidOperationException("La hora de fin debe ser mayor a la hora de inicio");

            var result = await _scheduleRepository.UpdateAsync(existingSchedule);

            if (!result)
                throw new InvalidOperationException("No se pudo actualizar el horario");

            return _mapper.Map<ProfessionalScheduleResponseDto>(existingSchedule);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);

            if (schedule == null)
                throw new KeyNotFoundException("No se encontró el horario");

            var result = await _scheduleRepository.RemoveAsync(schedule);

            if (!result)
                throw new InvalidOperationException("No se pudo eliminar el horario");

            return true;
        }

        public async Task<PagedResult<ProfessionalScheduleResponseDto>> FindAsync(ProfessionalScheduleFilterDto filter)
        {
            var query = await _scheduleRepository.FindAsync(h =>
                (!filter.ProfessionalId.HasValue || h.ProfessionalId == filter.ProfessionalId.Value) &&
                (!filter.DayOfWeek.HasValue || h.DayOfWeek == filter.DayOfWeek.Value)
            );

            var totalCount = query.Count();

            var orderedQuery = filter.OrderBy?.ToLower() switch
            {
                "starttime" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(h => h.StartTime)
                    : query.OrderBy(h => h.StartTime),
                _ => query.OrderBy(h => h.DayOfWeek).ThenBy(h => h.StartTime)
            };

            var paginatedSchedules = orderedQuery
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var schedulesDto = _mapper.Map<List<ProfessionalScheduleResponseDto>>(paginatedSchedules);

            return new PagedResult<ProfessionalScheduleResponseDto>
            {
                Items = schedulesDto,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
    }
}