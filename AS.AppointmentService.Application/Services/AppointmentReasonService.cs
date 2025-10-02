using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using AutoMapper;

namespace AS.AppointmentService.Application.Services
{
    public class AppointmentReasonService : IAppointmentReasonService
    {
        private readonly IGenericRepository<AppointmentReason> _reasonRepository;
        private readonly IMapper _mapper;

        public AppointmentReasonService(
            IGenericRepository<AppointmentReason> reasonRepository,
            IMapper mapper)
        {
            _reasonRepository = reasonRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentReasonResponseDto?> GetByIdAsync(int id)
        {
            var reason = await _reasonRepository.GetByIdAsync(id);

            if (reason == null)
                return null;

            return _mapper.Map<AppointmentReasonResponseDto>(reason);
        }

        public async Task<List<AppointmentReasonResponseDto>> GetAllAsync()
        {
            var reasons = await _reasonRepository.GetAllAsync();
            return _mapper.Map<List<AppointmentReasonResponseDto>>(reasons);
        }
    }
}