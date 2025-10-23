using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.SlotGenerationConfig;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    /// <summary>
    /// Gestión de configuración de generación de slots por profesional
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SlotGenerationConfigController : ControllerBase
    {
        private readonly ISlotGenerationConfigService _service;
        private readonly IAppLogger<SlotGenerationConfigController> _logger;

        public SlotGenerationConfigController(
            ISlotGenerationConfigService service,
            IAppLogger<SlotGenerationConfigController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene una configuración por su ID
        /// </summary>
        /// <param name="id">ID de la configuración</param>
        /// <returns>Configuración encontrada</returns>
        /// <response code="200">Configuración encontrada exitosamente</response>
        /// <response code="404">Configuración no encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseApi<SlotGenerationConfigResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseApi<SlotGenerationConfigResponseDto>();
            try
            {
                var config = await _service.GetByIdAsync(id);

                if (config == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró la configuración";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = config;
                response.Message = "Configuración encontrada";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("SlotGenerationConfig GetById", ex, id);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene la configuración de un profesional específico
        /// </summary>
        /// <param name="professionalId">ID del profesional</param>
        /// <returns>Configuración del profesional</returns>
        /// <response code="200">Configuración obtenida exitosamente (puede ser null si no existe)</response>
        /// <remarks>
        /// Si el profesional no tiene configuración personalizada, retorna null.
        /// En ese caso, se aplican los valores por defecto del sistema.
        /// </remarks>
        [HttpGet("professional/{professionalId}")]
        [ProducesResponseType(typeof(ResponseApi<SlotGenerationConfigResponseDto>), 200)]
        public async Task<IActionResult> GetByProfessionalId(Guid professionalId)
        {
            var response = new ResponseApi<SlotGenerationConfigResponseDto>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetByProfessionalIdAsync(professionalId);
                response.Message = response.Data != null
                    ? "Configuración encontrada"
                    : "No hay configuración personalizada (se usan valores por defecto)";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("SlotGenerationConfig GetByProfessionalId", ex, professionalId);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene todas las configuraciones del sistema
        /// </summary>
        /// <returns>Lista de todas las configuraciones</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        [HttpGet]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<List<SlotGenerationConfigResponseDto>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<SlotGenerationConfigResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetAllAsync();
                response.Message = "Configuraciones obtenidas exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("SlotGenerationConfig GetAll", ex);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene configuraciones con generación automática habilitada
        /// </summary>
        /// <returns>Lista de configuraciones con auto-generación activa</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        /// <remarks>
        /// Útil para procesos batch que generan slots automáticamente.
        /// Retorna solo profesionales que tienen habilitada la generación automática.
        /// </remarks>
        [HttpGet("auto-generate-enabled")]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<List<SlotGenerationConfigResponseDto>>), 200)]
        public async Task<IActionResult> GetAutoGenerateEnabled()
        {
            var response = new ResponseApi<List<SlotGenerationConfigResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetAutoGenerateEnabledAsync();
                response.Message = "Configuraciones con auto-generación obtenidas exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("SlotGenerationConfig GetAutoGenerateEnabled", ex);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Crea una nueva configuración para un profesional
        /// </summary>
        /// <param name="dto">Datos de la configuración</param>
        /// <returns>Configuración creada</returns>
        /// <response code="201">Configuración creada exitosamente</response>
        /// <response code="400">Datos inválidos o ya existe configuración para el profesional</response>
        /// <remarks>
        /// Cada profesional puede tener solo una configuración.
        /// Si ya existe, usar el endpoint de actualización.
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/slotgenerationconfig
        ///     {
        ///         "professionalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "advanceBookingDays": 60,
        ///         "minAdvanceHours": 24,
        ///         "allowSameDayBooking": false,
        ///         "autoGenerateSlots": true,
        ///         "maxAppointmentsPerDay": 20,
        ///         "bufferTimeMins": 5
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<SlotGenerationConfigResponseDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateSlotGenerationConfigDto dto)
        {
            var response = new ResponseApi<SlotGenerationConfigResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);

                response.IsSuccess = true;
                response.Data = created;
                response.Message = "Configuración creada exitosamente";

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError("SlotGenerationConfig Create - Validation", ex, dto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("SlotGenerationConfig Create", ex, dto);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Actualiza una configuración existente por ID
        /// </summary>
        /// <param name="id">ID de la configuración</param>
        /// <param name="dto">Datos a actualizar</param>
        /// <returns>Configuración actualizada</returns>
        /// <response code="200">Configuración actualizada exitosamente</response>
        /// <response code="404">Configuración no encontrada</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<SlotGenerationConfigResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSlotGenerationConfigDto dto)
        {
            var response = new ResponseApi<SlotGenerationConfigResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                response.IsSuccess = true;
                response.Data = updated;
                response.Message = "Configuración actualizada exitosamente";
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
                _logger.LogError("SlotGenerationConfig Update", ex, dto);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Actualiza la configuración de un profesional específico
        /// </summary>
        /// <param name="professionalId">ID del profesional</param>
        /// <param name="dto">Datos a actualizar</param>
        /// <returns>Configuración actualizada</returns>
        /// <response code="200">Configuración actualizada exitosamente</response>
        /// <response code="404">Configuración no encontrada para el profesional</response>
        /// <remarks>
        /// Endpoint alternativo para actualizar usando el ID del profesional en lugar del ID de configuración.
        /// Útil cuando se conoce el profesional pero no el ID de su configuración.
        /// </remarks>
        [HttpPut("professional/{professionalId}")]
        [Authorize(Roles = "Admin,ScheduleManager,Professional")]
        [ProducesResponseType(typeof(ResponseApi<SlotGenerationConfigResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateByProfessionalId(Guid professionalId, [FromBody] UpdateSlotGenerationConfigDto dto)
        {
            var response = new ResponseApi<SlotGenerationConfigResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateByProfessionalIdAsync(professionalId, dto);

                response.IsSuccess = true;
                response.Data = updated;
                response.Message = "Configuración actualizada exitosamente";
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
                _logger.LogError("SlotGenerationConfig UpdateByProfessionalId", ex, dto);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Elimina una configuración
        /// </summary>
        /// <param name="id">ID de la configuración a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Configuración eliminada exitosamente</response>
        /// <response code="404">Configuración no encontrada</response>
        /// <remarks>
        /// Al eliminar la configuración, el profesional volverá a usar los valores por defecto del sistema.
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.DeleteAsync(id);
                response.Message = "Configuración eliminada exitosamente";
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
                _logger.LogError("SlotGenerationConfig Delete", ex, id);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Verifica si se puede reservar un turno según la configuración del profesional
        /// </summary>
        /// <param name="professionalId">ID del profesional</param>
        /// <param name="appointmentDate">Fecha del turno (formato: YYYY-MM-DD)</param>
        /// <param name="appointmentTime">Hora del turno (formato: HH:mm)</param>
        /// <returns>True si se puede reservar, False si no cumple las reglas</returns>
        /// <response code="200">Verificación exitosa</response>
        /// <remarks>
        /// Valida:
        /// - Días máximos de anticipación
        /// - Horas mínimas de anticipación
        /// - Si permite reservas el mismo día
        /// 
        /// Ejemplo: GET /api/slotgenerationconfig/can-book/3fa85f64.../2025-10-15/14:30
        /// </remarks>
        [HttpGet("can-book/{professionalId}/{appointmentDate}/{appointmentTime}")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        public async Task<IActionResult> CanBookAppointment(Guid professionalId, DateOnly appointmentDate, TimeOnly appointmentTime)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.CanBookAppointmentAsync(professionalId, appointmentDate, appointmentTime);
                response.Message = response.Data
                    ? "Se puede reservar el turno"
                    : "No se puede reservar: no cumple con las reglas de anticipación";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("SlotGenerationConfig CanBookAppointment", ex, new { professionalId, appointmentDate, appointmentTime });
                return BadRequest(response);
            }
        }
    }
}