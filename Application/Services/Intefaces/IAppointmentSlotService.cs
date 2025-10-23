using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.AppointmentSlot;

namespace AS.AppointmentService.Application.Services.Interfaces
{
    public interface IAppointmentSlotService
    {
        Task<AppointmentSlotResponseDto?> GetByIdAsync(Guid id);
        Task<List<AppointmentSlotResponseDto>> GetAvailableByProfessionalAsync(Guid professionalId, DateOnly date);
        Task<List<AppointmentSlotResponseDto>> GetByProfessionalAndDateRangeAsync(Guid professionalId, DateOnly dateFrom, DateOnly dateTo);
        Task<List<AppointmentSlotResponseDto>> GetAllAsync();
        Task<AppointmentSlotResponseDto> CreateAsync(CreateAppointmentSlotDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<PagedResult<AppointmentSlotResponseDto>> FindAsync(AppointmentSlotFilterDto filter);
        Task<int> GenerateSlotsAsync(GenerateSlotsDto dto);
        Task<bool> BookSlotAsync(Guid slotId, Guid appointmentId);
        Task<bool> ReleaseSlotAsync(Guid slotId);
        Task<int> DeleteOldSlotsAsync(DateOnly beforeDate);
    }
}