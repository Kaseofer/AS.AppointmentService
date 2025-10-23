/*
 * APPOINTMENT STATUS ENDPOINTS
 * 
 * GET /api/appointmentstatus        - Obtener todos los estados de cita
 * GET /api/appointmentstatus/{id}   - Obtener un estado de cita por ID
 * 
 * Nota: Controller de solo lectura (catálogo)
 * Los estados de cita son valores fijos del sistema
 */

using AS.AppointmentService.Application.Dtos.Appointment;

namespace AS.AppointmentService.Application.Services.Interfaces
{
    public interface IAppointmentStatusService
    {
        Task<AppointmentStatusResponseDto?> GetByIdAsync(int id);
        Task<List<AppointmentStatusResponseDto>> GetAllAsync();
    }
}