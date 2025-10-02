using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.AppointmentSlot;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using System;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class AppointmentSlotService : IAppointmentSlotService
    {
        private readonly IGenericRepository<AppointmentSlots> _repository;
        private readonly IGenericRepository<ProfessionalSchedule> _scheduleRepository;
        private readonly IGenericRepository<ProfessionalNonWorkingDay> _nonWorkingDayRepository;
        private readonly IGenericRepository<NationalHoliday> _holidayRepository;
        private readonly IMapper _mapper;

        public AppointmentSlotService(
            IGenericRepository<AppointmentSlots> repository,
            IGenericRepository<ProfessionalSchedule> scheduleRepository,
            IGenericRepository<ProfessionalNonWorkingDay> nonWorkingDayRepository,
            IGenericRepository<NationalHoliday> holidayRepository,
            IMapper mapper)
        {
            _repository = repository;
            _scheduleRepository = scheduleRepository;
            _nonWorkingDayRepository = nonWorkingDayRepository;
            _holidayRepository = holidayRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentSlotResponseDto?> GetByIdAsync(Guid id)
        {
            var slot = await _repository.GetByIdAsync(id);

            if (slot == null)
                throw new KeyNotFoundException("No se encontró el slot");

            return _mapper.Map<AppointmentSlotResponseDto>(slot);
        }

        public async Task<List<AppointmentSlotResponseDto>> GetAvailableByProfessionalAsync(Guid professionalId, DateOnly date)
        {
            var slots = await _repository.FindAsync(s =>
                s.ProfessionalId == professionalId &&
                s.Date == date &&
                s.IsAvailable
            );

            return _mapper.Map<List<AppointmentSlotResponseDto>>(
                slots.OrderBy(s => s.StartTime).ToList()
            );
        }

        public async Task<List<AppointmentSlotResponseDto>> GetByProfessionalAndDateRangeAsync(
            Guid professionalId,
            DateOnly dateFrom,
            DateOnly dateTo)
        {
            var slots = await _repository.FindAsync(s =>
                s.ProfessionalId == professionalId &&
                s.Date >= dateFrom &&
                s.Date <= dateTo
            );

            return _mapper.Map<List<AppointmentSlotResponseDto>>(
                slots.OrderBy(s => s.Date).ThenBy(s => s.StartTime).ToList()
            );
        }

        public async Task<List<AppointmentSlotResponseDto>> GetAllAsync()
        {
            var slots = await _repository.GetAllAsync();
            return _mapper.Map<List<AppointmentSlotResponseDto>>(slots);
        }

        public async Task<AppointmentSlotResponseDto> CreateAsync(CreateAppointmentSlotDto dto)
        {
            // Validar que EndTime > StartTime
            if (dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("La hora de fin debe ser mayor a la hora de inicio");

            // Verificar que no exista un slot en el mismo horario
            var existing = await _repository.FindAsync(s =>
                s.ProfessionalId == dto.ProfessionalId &&
                s.Date == dto.Date &&
                s.StartTime == dto.StartTime
            );

            if (existing.Any())
                throw new InvalidOperationException($"Ya existe un slot en {dto.Date:dd/MM/yyyy} a las {dto.StartTime:HH:mm}");

            var slot = _mapper.Map<AppointmentSlots>(dto);
            slot.GeneratedAt = DateTime.UtcNow;
            slot.IsAvailable = true;

            var created = await _repository.AddAsync(slot);

            if (created.Id == Guid.Empty)
                throw new InvalidOperationException("No se pudo crear el slot");

            return _mapper.Map<AppointmentSlotResponseDto>(created);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var slot = await _repository.GetByIdAsync(id);

            if (slot == null)
                throw new KeyNotFoundException("No se encontró el slot");

            if (!slot.IsAvailable)
                throw new InvalidOperationException("No se puede eliminar un slot que está ocupado");

            var result = await _repository.RemoveAsync(slot);

            if (!result)
                throw new InvalidOperationException("No se pudo eliminar el slot");

            return true;
        }

        public async Task<PagedResult<AppointmentSlotResponseDto>> FindAsync(AppointmentSlotFilterDto filter)
        {
            var query = await _repository.FindAsync(s =>
                (!filter.ProfessionalId.HasValue || s.ProfessionalId == filter.ProfessionalId.Value) &&
                (!filter.DateFrom.HasValue || s.Date >= filter.DateFrom.Value) &&
                (!filter.DateTo.HasValue || s.Date <= filter.DateTo.Value) &&
                (!filter.IsAvailable.HasValue || s.IsAvailable == filter.IsAvailable.Value) &&
                (!filter.TimeFrom.HasValue || s.StartTime >= filter.TimeFrom.Value) &&
                (!filter.TimeTo.HasValue || s.StartTime <= filter.TimeTo.Value)
            );

            var totalCount = query.Count();

            var orderedQuery = filter.OrderBy?.ToLower() switch
            {
                "starttime" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(s => s.StartTime)
                    : query.OrderBy(s => s.StartTime),
                _ => query.OrderBy(s => s.Date).ThenBy(s => s.StartTime)
            };

            var paginated = orderedQuery
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var dtos = _mapper.Map<List<AppointmentSlotResponseDto>>(paginated);

            return new PagedResult<AppointmentSlotResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<int> GenerateSlotsAsync(GenerateSlotsDto dto)
        {
            if (dto.DateTo < dto.DateFrom)
                throw new InvalidOperationException("La fecha hasta debe ser mayor o igual a la fecha desde");

            // Obtener horarios del profesional
            var schedules = await _scheduleRepository.FindAsync(s =>
                s.ProfessionalId == dto.ProfessionalId && s.Active);

            if (!schedules.Any())
                throw new InvalidOperationException("El profesional no tiene horarios configurados");

            // Obtener días no laborales
            var nonWorkingDays = await _nonWorkingDayRepository.FindAsync(nw =>
                nw.ProfessionalId == dto.ProfessionalId &&
                nw.Date >= dto.DateFrom &&
                nw.Date <= dto.DateTo
            );

            // Obtener feriados
            var holidays = await _holidayRepository.FindAsync(h =>
                h.Date >= dto.DateFrom &&
                h.Date <= dto.DateTo &&
                h.Active
            );

            int slotsCreated = 0;

            // Generar slots día por día
            for (var date = dto.DateFrom; date <= dto.DateTo; date = date.AddDays(1))
            {
                // Saltar si es feriado
                if (holidays.Any(h => h.Date == date))
                    continue;

                // Saltar si es día no laboral
                if (nonWorkingDays.Any(nw => nw.Date == date && nw.AllDay))
                    continue;

                // Obtener horario del día
                var dayOfWeek = (int)date.DayOfWeek;
                var daySchedules = schedules.Where(s => s.DayOfWeek == dayOfWeek).ToList();

                foreach (var schedule in daySchedules)
                {
                    // Convertir ambos TimeSpan a TimeOnly
                    TimeOnly currentTime = TimeOnly.FromTimeSpan(schedule.StartTime);
                    TimeOnly endTime = TimeOnly.FromTimeSpan(schedule.EndTime);

                    while (currentTime.AddMinutes(schedule.AppointmentDurationMins) <= endTime)
                    {
                        // Verificar si ya existe el slot
                        var existing = await _repository.FindAsync(s =>
                            s.ProfessionalId == dto.ProfessionalId &&
                            s.Date == date &&
                            s.StartTime == currentTime
                        );

                        if (!existing.Any())
                        {
                            var slot = new AppointmentSlots
                            {
                                Id = Guid.NewGuid(),
                                ProfessionalId = dto.ProfessionalId,
                                Date = date,
                                StartTime = currentTime,
                                EndTime = currentTime.AddMinutes(schedule.AppointmentDurationMins),
                                DurationMins = schedule.AppointmentDurationMins,
                                IsAvailable = true,
                                GeneratedAt = DateTime.UtcNow
                            };

                            await _repository.AddAsync(slot);
                            slotsCreated++;
                        }

                        currentTime = currentTime.AddMinutes(schedule.AppointmentDurationMins);
                    }
                }
            }

            return slotsCreated;
        }

        public async Task<bool> BookSlotAsync(Guid slotId, Guid appointmentId)
        {
            var slot = await _repository.GetByIdAsync(slotId);

            if (slot == null)
                throw new KeyNotFoundException("No se encontró el slot");

            if (!slot.IsAvailable)
                throw new InvalidOperationException("El slot no está disponible");

            slot.IsAvailable = false;
            slot.AppointmentId = appointmentId;
            slot.BookedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(slot);
        }

        public async Task<bool> ReleaseSlotAsync(Guid slotId)
        {
            var slot = await _repository.GetByIdAsync(slotId);

            if (slot == null)
                throw new KeyNotFoundException("No se encontró el slot");

            slot.IsAvailable = true;
            slot.AppointmentId = null;
            slot.BookedAt = null;

            return await _repository.UpdateAsync(slot);
        }

        public async Task<int> DeleteOldSlotsAsync(DateOnly beforeDate)
        {
            var oldSlots = await _repository.FindAsync(s =>
                s.Date < beforeDate &&
                s.IsAvailable
            );

            int deleted = 0;
            foreach (var slot in oldSlots)
            {
                if (await _repository.RemoveAsync(slot))
                    deleted++;
            }

            return deleted;
        }
    }
}