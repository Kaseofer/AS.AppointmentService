using AS.AppointmentService.Application.Dtos.SlotGenerationConfig;

namespace AS.AppointmentService.Application.Services.Interfaces
{
    public interface ISlotGenerationConfigService
    {
        Task<SlotGenerationConfigResponseDto?> GetByIdAsync(int id);
        Task<SlotGenerationConfigResponseDto?> GetByProfessionalIdAsync(Guid professionalId);
        Task<List<SlotGenerationConfigResponseDto>> GetAllAsync();
        Task<List<SlotGenerationConfigResponseDto>> GetAutoGenerateEnabledAsync();
        Task<SlotGenerationConfigResponseDto> CreateAsync(CreateSlotGenerationConfigDto dto);
        Task<SlotGenerationConfigResponseDto> UpdateAsync(int id, UpdateSlotGenerationConfigDto dto);
        Task<SlotGenerationConfigResponseDto> UpdateByProfessionalIdAsync(Guid professionalId, UpdateSlotGenerationConfigDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> CanBookAppointmentAsync(Guid professionalId, DateOnly appointmentDate, TimeOnly appointmentTime);
    }
}