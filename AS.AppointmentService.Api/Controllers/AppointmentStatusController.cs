/*
 * APPOINTMENT STATUS ENDPOINTS
 * 
 * GET /api/appointmentstatus        - Obtener todos los estados de cita
 * GET /api/appointmentstatus/{id}   - Obtener un estado de cita por ID
 * 
 * Nota: Controller de solo lectura (catálogo)
 * Los estados de cita son valores fijos del sistema
 */

using AgendaSaludApp.Application.Common;
using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentStatusController : ControllerBase
    {
        private readonly IAppointmentStatusService _appointmentStatusService;
        private readonly IAppLogger<AppointmentStatusController> _logger;

        public AppointmentStatusController(
            IAppointmentStatusService appointmentStatusService,
            IAppLogger<AppointmentStatusController> logger)
        {
            _appointmentStatusService = appointmentStatusService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseApi<AppointmentStatusResponseDto>();
            try
            {
                var status = await _appointmentStatusService.GetByIdAsync(id);

                if (status == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró el estado de cita";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = status;
                response.Message = "Estado de cita encontrado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentStatus GetById", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<AppointmentStatusResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _appointmentStatusService.GetAllAsync();
                response.Message = "Estados de cita obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentStatus GetAll", ex);
                return BadRequest(response);
            }
        }
    }
}