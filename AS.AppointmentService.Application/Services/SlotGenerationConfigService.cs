using AS.AppointmentService.Application.Dtos.SlotGenerationConfig;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class SlotGenerationConfigService : ISlotGenerationConfigService
    {
        private readonly IGenericRepository<SlotGenerationConfig> _repository;
        private readonly IMapper _mapper;

        public SlotGenerationConfigService(
            IGenericRepository<SlotGenerationConfig> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SlotGenerationConfigResponseDto?> GetByIdAsync(int id)
        {
            var config = await _repository.GetByIdAsync(id);

            if (config == null)
                throw new KeyNotFoundException("No se encontró la configuración");

            return _mapper.Map<SlotGenerationConfigResponseDto>(config);
        }

        public async Task<SlotGenerationConfigResponseDto?> GetByProfessionalIdAsync(Guid professionalId)
        {
            var configs = await _repository.FindAsync(c => c.ProfessionalId == professionalId);
            var config = configs.FirstOrDefault();

            return config != null ? _mapper.Map<SlotGenerationConfigResponseDto>(config) : null;
        }

        public async Task<List<SlotGenerationConfigResponseDto>> GetAllAsync()
        {
            var configs = await _repository.GetAllAsync();
            return _mapper.Map<List<SlotGenerationConfigResponseDto>>(configs);
        }

        public async Task<List<SlotGenerationConfigResponseDto>> GetAutoGenerateEnabledAsync()
        {
            var configs = await _repository.FindAsync(c => c.AutoGenerateSlots);
            return _mapper.Map<List<SlotGenerationConfigResponseDto>>(configs.ToList());
        }

        public async Task<SlotGenerationConfigResponseDto> CreateAsync(CreateSlotGenerationConfigDto dto)
        {
            // Verificar que no exista ya una configuración para este profesional
            var existing = await _repository.FindAsync(c => c.ProfessionalId == dto.ProfessionalId);
            if (existing.Any())
                throw new InvalidOperationException("Ya existe una configuración para este profesional");

            var config = _mapper.Map<SlotGenerationConfig>(dto);
            config.CreatedAt = DateTime.UtcNow;
            config.UpdatedAt = DateTime.UtcNow;

            var created = await _repository.AddAsync(config);

            if (created.Id == 0)
                throw new InvalidOperationException("No se pudo crear la configuración");

            return _mapper.Map<SlotGenerationConfigResponseDto>(created);
        }

        public async Task<SlotGenerationConfigResponseDto> UpdateAsync(int id, UpdateSlotGenerationConfigDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException("No se encontró la configuración");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _repository.UpdateAsync(existing);

            if (!result)
                throw new InvalidOperationException("No se pudo actualizar la configuración");

            return _mapper.Map<SlotGenerationConfigResponseDto>(existing);
        }

        public async Task<SlotGenerationConfigResponseDto> UpdateByProfessionalIdAsync(
            Guid professionalId,
            UpdateSlotGenerationConfigDto dto)
        {
            var configs = await _repository.FindAsync(c => c.ProfessionalId == professionalId);
            var existing = configs.FirstOrDefault();

            if (existing == null)
                throw new KeyNotFoundException("No se encontró la configuración para este profesional");

            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;

            var result = await _repository.UpdateAsync(existing);

            if (!result)
                throw new InvalidOperationException("No se pudo actualizar la configuración");

            return _mapper.Map<SlotGenerationConfigResponseDto>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var config = await _repository.GetByIdAsync(id);

            if (config == null)
                throw new KeyNotFoundException("No se encontró la configuración");

            var result = await _repository.RemoveAsync(config);

            if (!result)
                throw new InvalidOperationException("No se pudo eliminar la configuración");

            return true;
        }

        public async Task<bool> CanBookAppointmentAsync(Guid professionalId, DateOnly appointmentDate, TimeOnly appointmentTime)
        {
            var configs = await _repository.FindAsync(c => c.ProfessionalId == professionalId);
            var config = configs.FirstOrDefault();

            // Si no hay configuración, usar valores por defecto
            if (config == null)
                return true;

            var now = DateTime.Now;
            var appointmentDateTime = appointmentDate.ToDateTime(appointmentTime);

            // Verificar si permite reservar el mismo día
            if (!config.AllowSameDayBooking && appointmentDate == DateOnly.FromDateTime(now))
                return false;

            // Verificar horas mínimas de anticipación
            var hoursUntilAppointment = (appointmentDateTime - now).TotalHours;
            if (hoursUntilAppointment < config.MinAdvanceHours)
                return false;

            // Verificar días máximos de anticipación
            var daysUntilAppointment = (appointmentDate.ToDateTime(TimeOnly.MinValue) - DateOnly.FromDateTime(now).ToDateTime(TimeOnly.MinValue)).Days;
            if (daysUntilAppointment > config.AdvanceBookingDays)
                return false;

            return true;
        }
    }
}