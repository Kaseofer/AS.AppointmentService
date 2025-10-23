using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.ProfessionalNonWorkingDay;

namespace AS.AppointmentService.Application.Services.Interfaces
{
    public interface IProfessionalNonWorkingDayService
    {
        Task<ProfessionalNonWorkingDayResponseDto?> GetByIdAsync(int id);
        Task<List<ProfessionalNonWorkingDayResponseDto>> GetByProfessionalIdAsync(Guid professionalId);
        Task<List<ProfessionalNonWorkingDayResponseDto>> GetByDateRangeAsync(Guid professionalId, DateOnly dateFrom, DateOnly dateTo);
        Task<List<ProfessionalNonWorkingDayResponseDto>> GetAllAsync();
        Task<ProfessionalNonWorkingDayResponseDto> CreateAsync(CreateProfessionalNonWorkingDayDto dto, Guid? createdBy);
        Task<ProfessionalNonWorkingDayResponseDto> UpdateAsync(int id, UpdateProfessionalNonWorkingDayDto dto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<ProfessionalNonWorkingDayResponseDto>> FindAsync(ProfessionalNonWorkingDayFilterDto filter);
        Task<bool> IsNonWorkingDayAsync(Guid professionalId, DateOnly date, TimeOnly? time = null);
    }
}