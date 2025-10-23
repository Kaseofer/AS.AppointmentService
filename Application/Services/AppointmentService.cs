using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IGenericRepository<Appointment> _appointmentRepository;
        private readonly IGenericRepository<AppointmentStatus> _statusRepository;
        private readonly IGenericRepository<AppointmentReason> _reasonRepository;
        private readonly IMapper _mapper;

        public AppointmentService(
            IGenericRepository<Appointment> appointmentRepository,
            IGenericRepository<AppointmentStatus> statusRepository,
            IGenericRepository<AppointmentReason> reasonRepository,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _statusRepository = statusRepository;
            _reasonRepository = reasonRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentResponseDto?> GetByIdAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
                throw new KeyNotFoundException("No se encontró la cita");

            return _mapper.Map<AppointmentResponseDto>(appointment);
        }

        public async Task<List<AppointmentResponseDto>> GetAllAsync()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return _mapper.Map<List<AppointmentResponseDto>>(appointments);
        }

        public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto)
        {
            // Validar que EndTime > StartTime
            if (dto.EndTime <= dto.StartTime)
                throw new InvalidOperationException("La hora de fin debe ser mayor a la hora de inicio");

            // Validar que el motivo existe
            var reason = await _reasonRepository.GetByIdAsync(dto.ReasonId);
            if (reason == null)
                throw new KeyNotFoundException("No se encontró el motivo de cita");

            // Validar que no haya solapamiento de citas para el mismo profesional
            var existingAppointments = await _appointmentRepository.FindAsync(a =>
                a.ProfessionalId == dto.ProfessionalId &&
                a.Date == dto.Date &&
                a.IsBooked);

            foreach (var existing in existingAppointments)
            {
                if ((dto.StartTime >= existing.StartTime && dto.StartTime < existing.EndTime) ||
                    (dto.EndTime > existing.StartTime && dto.EndTime <= existing.EndTime) ||
                    (dto.StartTime <= existing.StartTime && dto.EndTime >= existing.EndTime))
                {
                    throw new InvalidOperationException(
                        $"Ya existe una cita en ese horario: {existing.StartTime:HH:mm} - {existing.EndTime:HH:mm}"
                    );
                }
            }

            var appointment = _mapper.Map<Appointment>(dto);

            // Establecer estado inicial (por ejemplo, "Pendiente" = 1)
            appointment.StatusId = 1;
            appointment.IsBooked = true;
            appointment.IsExpired = false;

            var createdAppointment = await _appointmentRepository.AddAsync(appointment);

            if (createdAppointment.Id == Guid.Empty)
                throw new InvalidOperationException("No se pudo crear la cita");

            return _mapper.Map<AppointmentResponseDto>(createdAppointment);
        }

        public async Task<AppointmentResponseDto> UpdateAsync(Guid id, UpdateAppointmentDto dto)
        {
            var existingAppointment = await _appointmentRepository.GetByIdAsync(id);

            if (existingAppointment == null)
                throw new KeyNotFoundException("No se encontró la cita");

            // Mapear cambios
            _mapper.Map(dto, existingAppointment);

            // Validar EndTime > StartTime si se modificaron
            if (existingAppointment.EndTime <= existingAppointment.StartTime)
                throw new InvalidOperationException("La hora de fin debe ser mayor a la hora de inicio");

            var result = await _appointmentRepository.UpdateAsync(existingAppointment);

            if (!result)
                throw new InvalidOperationException("No se pudo actualizar la cita");

            return _mapper.Map<AppointmentResponseDto>(existingAppointment);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
                throw new KeyNotFoundException("No se encontró la cita");

            var result = await _appointmentRepository.RemoveAsync(appointment);

            if (!result)
                throw new InvalidOperationException("No se pudo eliminar la cita");

            return true;
        }

        public async Task<PagedResult<AppointmentResponseDto>> FindAsync(AppointmentFilterDto filter)
        {
            var query = await _appointmentRepository.FindAsync(a =>
                (!filter.ProfessionalId.HasValue || a.ProfessionalId == filter.ProfessionalId.Value) &&
                (!filter.PatientId.HasValue || a.PatientId == filter.PatientId.Value) &&
                (!filter.DateFrom.HasValue || a.Date >= filter.DateFrom.Value) &&
                (!filter.DateTo.HasValue || a.Date <= filter.DateTo.Value) &&
                (!filter.StatusId.HasValue || a.StatusId == filter.StatusId.Value) &&
                (!filter.IsBooked.HasValue || a.IsBooked == filter.IsBooked.Value) &&
                (!filter.IsExpired.HasValue || a.IsExpired == filter.IsExpired.Value)
            );

            var totalCount = query.Count();

            var orderedQuery = filter.OrderBy?.ToLower() switch
            {
                "starttime" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(a => a.StartTime)
                    : query.OrderBy(a => a.StartTime),
                "professional" => filter.OrderDirection == "desc"
                    ? query.OrderByDescending(a => a.ProfessionalId)
                    : query.OrderBy(a => a.ProfessionalId),
                _ => query.OrderBy(a => a.Date).ThenBy(a => a.StartTime)
            };

            var paginatedAppointments = orderedQuery
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var appointmentsDto = _mapper.Map<List<AppointmentResponseDto>>(paginatedAppointments);

            return new PagedResult<AppointmentResponseDto>
            {
                Items = appointmentsDto,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<List<AppointmentResponseDto>> GetByProfessionalIdAsync(Guid professionalId)
        {
            var appointments = await _appointmentRepository.FindAsync(a => a.ProfessionalId == professionalId);
            return _mapper.Map<List<AppointmentResponseDto>>(
                appointments.OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToList()
            );
        }

        public async Task<List<AppointmentResponseDto>> GetByPatientIdAsync(Guid patientId)
        {
            var appointments = await _appointmentRepository.FindAsync(a => a.PatientId == patientId);
            return _mapper.Map<List<AppointmentResponseDto>>(
                appointments.OrderBy(a => a.Date).ThenBy(a => a.StartTime).ToList()
            );
        }

        public async Task<List<AppointmentResponseDto>> GetByDateAsync(DateOnly date)
        {
            var appointments = await _appointmentRepository.FindAsync(a => a.Date == date);
            return _mapper.Map<List<AppointmentResponseDto>>(
                appointments.OrderBy(a => a.StartTime).ToList()
            );
        }
    }
}