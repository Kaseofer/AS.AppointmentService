/*
 * APPOINTMENT REASON ENDPOINTS
 * 
 * GET /api/appointmentreason        - Obtener todos los motivos de cita
 * GET /api/appointmentreason/{id}   - Obtener un motivo de cita por ID
 * 
 * Nota: Controller de solo lectura (catálogo)
 * Los motivos de cita son valores configurables del sistema
 */

using AS.AppointmentService.Application.Dtos.Appointment;

namespace AS.AppointmentService.Application.Services.Interfaces
{
    public interface IAppointmentReasonService
    {
        Task<AppointmentReasonResponseDto?> GetByIdAsync(int id);
        Task<List<AppointmentReasonResponseDto>> GetAllAsync();
    }
}