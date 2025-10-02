using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.NationalHoliday;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AS.AppointmentService.Application.Dtos.NationalHoliday;
using AS.AppointmentService.Application.Services.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class NationalHolidayService : INationalHolidayService
    {
        private readonly IGenericRepository<NationalHoliday> _repository;
        private readonly IMapper _mapper;

        public NationalHolidayService(
            IGenericRepository<NationalHoliday> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<NationalHolidayResponseDto?> GetByIdAsync(int id)
        {
            var holiday = await _repository.GetByIdAsync(id);

            if (holiday == null)
                throw new KeyNotFoundException("No se encontró el feriado");

            return _mapper.Map<NationalHolidayResponseDto>(holiday);
        }

        public async Task<NationalHolidayResponseDto?> GetByDateAsync(DateOnly date)
        {
            var holidays = await _repository.FindAsync(h => h.Date == date && h.Active);
            var holiday = holidays.FirstOrDefault();

            return holiday != null ? _mapper.Map<NationalHolidayResponseDto>(holiday) : null;
        }

        public async Task<List<NationalHolidayResponseDto>> GetByYearAsync(int year)
        {
            var dateFrom = new DateOnly(year, 1, 1);
            var dateTo = new DateOnly(year, 12, 31);

            var holidays = await _repository.FindAsync(h =>
                h.Date >= dateFrom &&
                h.Date <= dateTo &&
                h.Active
            );

            return _mapper.Map<List<NationalHolidayResponseDto>>(
                holidays.OrderBy(h => h.Date).ToList()
            );
        }

        public async Task<List<NationalHolidayResponseDto>> GetAllAsync()
        {
            var holidays = await _repository.GetAllAsync();
            return _mapper.Map<List<NationalHolidayResponseDto>>(holidays);
        }

        public async Task<List<NationalHolidayResponseDto>> GetActiveAsync()
        {
            var holidays = await _repository.FindAsync(h => h.Active);
            return _mapper.Map<List<NationalHolidayResponseDto>>(
                holidays.OrderBy(h => h.Date).ToList()
            );
        }

        public async Task<NationalHolidayResponseDto> CreateAsync(CreateNationalHolidayDto dto)
        {
            // Verificar si ya existe un feriado en esa fecha
            var existing = await _repository.FindAsync(h => h.Date == dto.Date);
            if (existing.Any())
                throw new InvalidOperationException($"Ya existe un feriado registrado para el {dto.Date:dd/MM/yyyy}");

            var holiday = _mapper.Map<NationalHoliday>(dto);
            holiday.CreatedAt = DateTime.UtcNow;

            var created = await _repository.AddAsync(holiday);

            if (created.Id == 0)
                throw new InvalidOperationException("No se pudo crear el feriado");

            return _mapper.Map<NationalHolidayResponseDto>(created);
        }

        public async Task<NationalHolidayResponseDto> UpdateAsync(int id, UpdateNationalHolidayDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException("No se encontró el feriado");

            // Si se cambia la fecha, verificar que no exista otro feriado en esa fecha
            if (dto.Date.HasValue && dto.Date.Value != existing.Date)
            {
                var duplicates = await _repository.FindAsync(h => h.Date == dto.Date.Value && h.Id != id);
                if (duplicates.Any())
                    throw new InvalidOperationException($"Ya existe un feriado en la fecha {dto.Date.Value:dd/MM/yyyy}");
            }

            _mapper.Map(dto, existing);

            var result = await _repository.UpdateAsync(existing);

            if (!result)
                throw new InvalidOperationException("No se pudo actualizar el feriado");

            return _mapper.Map<NationalHolidayResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var holiday = await _repository.GetByIdAsync(id);

            if (holiday == null)
                throw new KeyNotFoundException("No se encontró el feriado");

            var result = await _repository.RemoveAsync(holiday);

            if (!result)
                throw new InvalidOperationException("No se pudo eliminar el feriado");

            return true;
        }

        public async Task<PagedResult<NationalHolidayResponseDto>> FindAsync(NationalHolidayFilterDto filter)
        {
            var query = await _repository.FindAsync(h =>
                (!filter.Year.HasValue || h.Date.Year == filter.Year.Value) &&
                (!filter.Month.HasValue || h.Date.Month == filter.Month.Value) &&
                (!filter.DateFrom.HasValue || h.Date >= filter.DateFrom.Value) &&
                (!filter.DateTo.HasValue || h.Date <= filter.DateTo.Value) &&
                (!filter.IsRecurring.HasValue || h.IsRecurring == filter.IsRecurring.Value) &&
                (!filter.Active.HasValue || h.Active == filter.Active.Value)
            );

            var totalCount = query.Count();

            var orderedQuery = filter.OrderBy?.ToLower() switch
            {
                "name" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(h => h.Name)
                    : query.OrderBy(h => h.Name),
                _ => query.OrderBy(h => h.Date)
            };

            var paginated = orderedQuery
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<NationalHolidayResponseDto>>(paginated);

            return new PagedResult<NationalHolidayResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<bool> IsHolidayAsync(DateOnly date)
        {
            var holidays = await _repository.FindAsync(h => h.Date == date && h.Active);
            return holidays.Any();
        }
    }
}