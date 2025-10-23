using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.NationalHoliday;

namespace AS.AppointmentService.Application.Services.Interfaces
{
    public interface INationalHolidayService
    {
        Task<NationalHolidayResponseDto?> GetByIdAsync(int id);
        Task<NationalHolidayResponseDto?> GetByDateAsync(DateOnly date);
        Task<List<NationalHolidayResponseDto>> GetByYearAsync(int year);
        Task<List<NationalHolidayResponseDto>> GetAllAsync();
        Task<List<NationalHolidayResponseDto>> GetActiveAsync();
        Task<NationalHolidayResponseDto> CreateAsync(CreateNationalHolidayDto dto);
        Task<NationalHolidayResponseDto> UpdateAsync(int id, UpdateNationalHolidayDto dto);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<NationalHolidayResponseDto>> FindAsync(NationalHolidayFilterDto filter);
        Task<bool> IsHolidayAsync(DateOnly date);
    }
}