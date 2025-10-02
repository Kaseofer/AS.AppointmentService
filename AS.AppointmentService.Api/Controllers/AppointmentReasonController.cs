/*
 * APPOINTMENT REASON ENDPOINTS
 * 
 * GET /api/appointmentreason        - Obtener todos los motivos de cita
 * GET /api/appointmentreason/{id}   - Obtener un motivo de cita por ID
 * 
 * Nota: Controller de solo lectura (catálogo)
 * Los motivos de cita son valores configurables del sistema
 */

using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentReasonController : ControllerBase
    {
        private readonly IAppointmentReasonService _appointmentReasonService;
        private readonly IAppLogger<AppointmentReasonController> _logger;

        public AppointmentReasonController(
            IAppointmentReasonService appointmentReasonService,
            IAppLogger<AppointmentReasonController> logger)
        {
            _appointmentReasonService = appointmentReasonService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseApi<AppointmentReasonResponseDto>();
            try
            {
                var reason = await _appointmentReasonService.GetByIdAsync(id);

                if (reason == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró el motivo de cita";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = reason;
                response.Message = "Motivo de cita encontrado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentReason GetById", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<AppointmentReasonResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _appointmentReasonService.GetAllAsync();
                response.Message = "Motivos de cita obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentReason GetAll", ex);
                return BadRequest(response);
            }
        }
    }
}