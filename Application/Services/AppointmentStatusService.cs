using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class AppointmentStatusService : IAppointmentStatusService
    {
        private readonly IGenericRepository<AppointmentStatus> _statusRepository;
        private readonly IMapper _mapper;

        public AppointmentStatusService(
            IGenericRepository<AppointmentStatus> statusRepository,
            IMapper mapper)
        {
            _statusRepository = statusRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentStatusResponseDto?> GetByIdAsync(int id)
        {
            var status = await _statusRepository.GetByIdAsync(id);

            if (status == null)
                return null;

            return _mapper.Map<AppointmentStatusResponseDto>(status);
        }

        public async Task<List<AppointmentStatusResponseDto>> GetAllAsync()
        {
            var statuses = await _statusRepository.GetAllAsync();
            return _mapper.Map<List<AppointmentStatusResponseDto>>(statuses);
        }
    }
}