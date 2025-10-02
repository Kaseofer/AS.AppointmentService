using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.Appointment;

namespace AS.AppointmentService.Application.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto?> GetByIdAsync(Guid id);
        Task<List<AppointmentResponseDto>> GetAllAsync();
        Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto);
        Task<AppointmentResponseDto> UpdateAsync(Guid id, UpdateAppointmentDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<PagedResult<AppointmentResponseDto>> FindAsync(AppointmentFilterDto filter);
        Task<List<AppointmentResponseDto>> GetByProfessionalIdAsync(Guid professionalId);
        Task<List<AppointmentResponseDto>> GetByPatientIdAsync(Guid patientId);
        Task<List<AppointmentResponseDto>> GetByDateAsync(DateOnly date);
    }
}