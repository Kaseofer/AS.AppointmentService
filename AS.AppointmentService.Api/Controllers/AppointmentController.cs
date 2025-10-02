using AgendaSaludApp.Application.Common;
using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Application.Services;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Core.Entities;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/*
 *APPOINTMENT ENDPOINTS
 *
 * GET    /api/appointment                    - Obtener todas las citas
 * GET    /api/appointment/{id}               -Obtener una cita por ID
 * GET    /api/appointment/search             - Buscar citas con filtros (profesional, paciente, fecha, estado, paginación)
 * POST   /api/appointment                    - Crear una nueva cita
 * PUT    /api/appointment/{id}               -Actualizar una cita existente
 * DELETE /api/appointment/{id}               -Eliminar una cita
 * 
 * Filtros disponibles en /search:
 *-ProfessionalId: Filtrar por profesional
 * - PatientId: Filtrar por paciente
 * - DateFrom/DateTo: Rango de fechas
 * - StatusId: Filtrar por estado
 * - IsBooked: Filtrar citas ocupadas/disponibles
 * - IsExpired: Filtrar citas vencidas
 * - PageNumber/PageSize: Paginación
 * -OrderBy / OrderDirection: Ordenamiento
 */

namespace AS.AppointmentService.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IAppLogger<AppointmentController> _logger;

        public AppointmentController(
            IAppointmentService appointmentService,
            IAppLogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = new ResponseApi<AppointmentResponseDto>();

            try
            {
                var appointment = await _appointmentService.GetByIdAsync(id);

                if (appointment == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró la cita";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = appointment;
                response.Message = "Cita encontrada";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("Appointment GetById", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<AppointmentResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _appointmentService.GetAllAsync();
                response.Message = "Lista de citas obtenida exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("Appointment GetAll", ex);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto appointmentDto)
        {
            var response = new ResponseApi<AppointmentResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var appointmentCreated = await _appointmentService.CreateAsync(appointmentDto);

                response.IsSuccess = true;
                response.Data = appointmentCreated;
                response.Message = "Cita creada exitosamente";

                return CreatedAtAction(nameof(GetById), new { id = appointmentCreated.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError("Appointment Create - Validation", ex, appointmentDto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("Appointment Create", ex, appointmentDto);
                return BadRequest(response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentDto appointmentDto)
        {
            var response = new ResponseApi<AppointmentResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var appointmentUpdated = await _appointmentService.UpdateAsync(id, appointmentDto);

                response.IsSuccess = true;
                response.Data = appointmentUpdated;
                response.Message = "Cita actualizada exitosamente";
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("Appointment Update", ex, appointmentDto);
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _appointmentService.DeleteAsync(id);
                response.Message = "Cita eliminada exitosamente";
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("Appointment Delete", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] AppointmentFilterDto filter)
        {
            var response = new ResponseApi<PagedResult<AppointmentResponseDto>>();

            try
            {
                var result = await _appointmentService.FindAsync(filter);

                response.IsSuccess = true;
                response.Data = result;
                response.Message = "Búsqueda exitosa";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("Appointment Search", ex, filter);
                return BadRequest(response);
            }
        }
    }
}