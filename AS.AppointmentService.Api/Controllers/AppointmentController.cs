using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    /// <summary>
    /// Gestión de citas médicas
    /// </summary>
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

        /// <summary>
        /// Obtiene una cita específica por su ID
        /// </summary>
        /// <param name="id">ID de la cita</param>
        /// <returns>Información completa de la cita</returns>
        /// <response code="200">Cita encontrada exitosamente</response>
        /// <response code="404">Cita no encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseApi<AppointmentResponseDto>), 200)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Obtiene todas las citas del sistema
        /// </summary>
        /// <returns>Lista completa de citas</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        /// <remarks>
        /// Endpoint administrativo. Para búsquedas específicas usar /search con filtros.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<List<AppointmentResponseDto>>), 200)]
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

        /// <summary>
        /// Crea una nueva cita médica
        /// </summary>
        /// <param name="appointmentDto">Datos de la cita a crear</param>
        /// <returns>Cita creada</returns>
        /// <response code="201">Cita creada exitosamente</response>
        /// <response code="400">Datos inválidos o conflicto de horarios</response>
        /// <remarks>
        /// Valida automáticamente:
        /// - Que no haya solapamiento de horarios para el profesional
        /// - Que el motivo de consulta exista
        /// - Que la hora de fin sea mayor a la hora de inicio
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/appointment
        ///     {
        ///         "professionalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
        ///         "date": "2025-10-15",
        ///         "startTime": "14:00",
        ///         "endTime": "14:30",
        ///         "reasonId": 1,
        ///         "observations": "Control de rutina"
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(ResponseApi<AppointmentResponseDto>), 201)]
        [ProducesResponseType(400)]
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

        /// <summary>
        /// Actualiza una cita existente
        /// </summary>
        /// <param name="id">ID de la cita a actualizar</param>
        /// <param name="appointmentDto">Datos a actualizar</param>
        /// <returns>Cita actualizada</returns>
        /// <response code="200">Cita actualizada exitosamente</response>
        /// <response code="404">Cita no encontrada</response>
        /// <response code="400">Datos inválidos o conflicto de horarios</response>
        /// <remarks>
        /// Permite modificar horarios, estado, observaciones, etc.
        /// Mantiene las mismas validaciones que la creación.
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ResponseApi<AppointmentResponseDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
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

        /// <summary>
        /// Elimina una cita
        /// </summary>
        /// <param name="id">ID de la cita a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Cita eliminada exitosamente</response>
        /// <response code="404">Cita no encontrada</response>
        /// <remarks>
        /// La eliminación es permanente. Se recomienda usar actualización de estado
        /// (ej: cambiar a "Cancelada") en lugar de eliminar para mantener historial.
        /// </remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        [ProducesResponseType(404)]
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

        /// <summary>
        /// Busca citas con filtros avanzados y paginación
        /// </summary>
        /// <param name="filter">Criterios de búsqueda y paginación</param>
        /// <returns>Resultado paginado de citas</returns>
        /// <response code="200">Búsqueda realizada exitosamente</response>
        /// <remarks>
        /// Filtros disponibles:
        /// - ProfessionalId: Filtrar por profesional específico
        /// - PatientId: Filtrar por paciente específico
        /// - DateFrom/DateTo: Rango de fechas
        /// - StatusId: Filtrar por estado de la cita (1=Pendiente, 2=Confirmada, 3=Cancelada, etc)
        /// - IsBooked: true=ocupadas, false=disponibles
        /// - IsExpired: true=vencidas, false=vigentes
        /// - PageNumber: Número de página (default: 1)
        /// - PageSize: Tamaño de página (default: 20)
        /// - OrderBy: Campo de ordenamiento (date, starttime, professional)
        /// - OrderDirection: Dirección de ordenamiento (asc, desc)
        /// 
        /// Ejemplo: /api/appointment/search?professionalId=xxx&amp;dateFrom=2025-10-01&amp;pageSize=10
        /// </remarks>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ResponseApi<PagedResult<AppointmentResponseDto>>), 200)]
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