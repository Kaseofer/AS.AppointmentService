using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.ProfessionalNonWorkingDay;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.UserManagement.Application.Services
{
    public class ProfessionalNonWorkingDayService : IProfessionalNonWorkingDayService
    {
        private readonly IGenericRepository<ProfessionalNonWorkingDay> _repository;
        private readonly IMapper _mapper;

        public ProfessionalNonWorkingDayService(
            IGenericRepository<ProfessionalNonWorkingDay> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProfessionalNonWorkingDayResponseDto?> GetByIdAsync(int id)
        {
            var nonWorkingDay = await _repository.GetByIdAsync(id);

            if (nonWorkingDay == null)
                throw new KeyNotFoundException("No se encontró el día no laboral");

            return _mapper.Map<ProfessionalNonWorkingDayResponseDto>(nonWorkingDay);
        }

        public async Task<List<ProfessionalNonWorkingDayResponseDto>> GetByProfessionalIdAsync(Guid professionalId)
        {
            var nonWorkingDays = await _repository.FindAsync(nw => nw.ProfessionalId == professionalId);
            return _mapper.Map<List<ProfessionalNonWorkingDayResponseDto>>(
                nonWorkingDays.OrderBy(nw => nw.Date).ToList()
            );
        }

        public async Task<List<ProfessionalNonWorkingDayResponseDto>> GetByDateRangeAsync(
            Guid professionalId,
            DateOnly dateFrom,
            DateOnly dateTo)
        {
            var nonWorkingDays = await _repository.FindAsync(nw =>
                nw.ProfessionalId == professionalId &&
                nw.Date >= dateFrom &&
                nw.Date <= dateTo
            );

            return _mapper.Map<List<ProfessionalNonWorkingDayResponseDto>>(
                nonWorkingDays.OrderBy(nw => nw.Date).ToList()
            );
        }

        public async Task<List<ProfessionalNonWorkingDayResponseDto>> GetAllAsync()
        {
            var nonWorkingDays = await _repository.GetAllAsync();
            return _mapper.Map<List<ProfessionalNonWorkingDayResponseDto>>(nonWorkingDays);
        }

        public async Task<ProfessionalNonWorkingDayResponseDto> CreateAsync(
            CreateProfessionalNonWorkingDayDto dto,
            Guid? createdBy)
        {


            // Validar reglas de negocio
            if (!dto.AllDay && (dto.StartTime == null || dto.EndTime == null))
                throw new InvalidOperationException("Si no es todo el día, debe especificar hora de inicio y fin");

            if (!dto.AllDay && dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("La hora de fin debe ser mayor a la hora de inicio");

            // Verificar si ya existe un registro para esa fecha
            var existing = await _repository.FindAsync(nw =>
                nw.ProfessionalId == dto.ProfessionalId &&
                nw.Date == dto.Date
            );

            if (existing.Any())
                throw new InvalidOperationException($"Ya existe un día no laboral registrado para el {dto.Date:dd/MM/yyyy}");

            var nonWorkingDay = _mapper.Map<ProfessionalNonWorkingDay>(dto);
            nonWorkingDay.CreatedBy = createdBy;
            nonWorkingDay.CreatedAt = DateTime.UtcNow;

            var created = await _repository.AddAsync(nonWorkingDay);

            if (created.Id == 0)
                throw new InvalidOperationException("No se pudo crear el día no laboral");

            return _mapper.Map<ProfessionalNonWorkingDayResponseDto>(created);
        }

        public async Task<ProfessionalNonWorkingDayResponseDto> UpdateAsync(int id, UpdateProfessionalNonWorkingDayDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException("No se encontró el día no laboral");

            // Mapear cambios
            _mapper.Map(dto, existing);

            // Validar reglas
            if (!existing.AllDay && existing.EndTime <= existing.StartTime)
                throw new InvalidOperationException("La hora de fin debe ser mayor a la hora de inicio");

            var result = await _repository.UpdateAsync(existing);

            if (!result)
                throw new InvalidOperationException("No se pudo actualizar el día no laboral");

            return _mapper.Map<ProfessionalNonWorkingDayResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var nonWorkingDay = await _repository.GetByIdAsync(id);

            if (nonWorkingDay == null)
                throw new KeyNotFoundException("No se encontró el día no laboral");

            var result = await _repository.RemoveAsync(nonWorkingDay);

            if (!result)
                throw new InvalidOperationException("No se pudo eliminar el día no laboral");

            return true;
        }

        public async Task<PagedResult<ProfessionalNonWorkingDayResponseDto>> FindAsync(ProfessionalNonWorkingDayFilterDto filter)
        {
            var query = await _repository.FindAsync(nw =>
                (!filter.ProfessionalId.HasValue || nw.ProfessionalId == filter.ProfessionalId.Value) &&
                (!filter.DateFrom.HasValue || nw.Date >= filter.DateFrom.Value) &&
                (!filter.DateTo.HasValue || nw.Date <= filter.DateTo.Value) &&
                (!filter.AllDay.HasValue || nw.AllDay == filter.AllDay.Value)
            );

            var totalCount = query.Count();

            var orderedQuery = filter.OrderBy?.ToLower() switch
            {
                "reason" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(nw => nw.Reason)
                    : query.OrderBy(nw => nw.Reason),
                _ => query.OrderBy(nw => nw.Date)
            };

            var paginated = orderedQuery
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<ProfessionalNonWorkingDayResponseDto>>(paginated);

            return new PagedResult<ProfessionalNonWorkingDayResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<bool> IsNonWorkingDayAsync(Guid professionalId, DateOnly date, TimeOnly? time = null)
        {
            var nonWorkingDays = await _repository.FindAsync(nw =>
                nw.ProfessionalId == professionalId &&
                nw.Date == date
            );

            if (!nonWorkingDays.Any())
                return false;

            foreach (var nw in nonWorkingDays)
            {
                // Si es todo el día, siempre es no laboral
                if (nw.AllDay)
                    return true;

                // Si tiene horario específico y se proporciona tiempo, verificar rango
                if (time.HasValue && nw.StartTime.HasValue && nw.EndTime.HasValue)
                {
                    if (time.Value >= nw.StartTime.Value && time.Value < nw.EndTime.Value)
                        return true;
                }
            }

            return false;
        }
    }
}