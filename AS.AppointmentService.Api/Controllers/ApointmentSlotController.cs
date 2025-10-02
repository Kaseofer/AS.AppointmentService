using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.AppointmentSlot;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    /// <summary>
    /// Gestión de slots de turnos disponibles
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentSlotController : ControllerBase
    {
        private readonly IAppointmentSlotService _service;
        private readonly IAppLogger<AppointmentSlotController> _logger;

        public AppointmentSlotController(
            IAppointmentSlotService service,
            IAppLogger<AppointmentSlotController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene un slot específico por su ID
        /// </summary>
        /// <param name="id">ID del slot</param>
        /// <returns>Información detallada del slot</returns>
        /// <response code="200">Slot encontrado exitosamente</response>
        /// <response code="404">Slot no encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseApi<AppointmentSlotResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = new ResponseApi<AppointmentSlotResponseDto>();
            try
            {
                var slot = await _service.GetByIdAsync(id);

                if (slot == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró el slot";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = slot;
                response.Message = "Slot encontrado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot GetById", ex, id);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene todos los slots disponibles de un profesional para una fecha específica
        /// </summary>
        /// <param name="professionalId">ID del profesional</param>
        /// <param name="date">Fecha a consultar (formato: YYYY-MM-DD)</param>
        /// <returns>Lista de slots disponibles ordenados por hora</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        /// <remarks>
        /// Este endpoint es útil para que los pacientes vean los horarios disponibles
        /// de un profesional en una fecha específica al agendar un turno.
        /// </remarks>
        [HttpGet("available/{professionalId}/{date}")]
        [ProducesResponseType(typeof(ResponseApi<List<AppointmentSlotResponseDto>>), 200)]
        public async Task<IActionResult> GetAvailableByProfessional(Guid professionalId, DateOnly date)
        {
            var response = new ResponseApi<List<AppointmentSlotResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetAvailableByProfessionalAsync(professionalId, date);
                response.Message = "Slots disponibles obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot GetAvailableByProfessional", ex, new { professionalId, date });
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene todos los slots de un profesional en un rango de fechas
        /// </summary>
        /// <param name="professionalId">ID del profesional</param>
        /// <param name="dateFrom">Fecha desde (formato: YYYY-MM-DD)</param>
        /// <param name="dateTo">Fecha hasta (formato: YYYY-MM-DD)</param>
        /// <returns>Lista de slots en el rango especificado</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        /// <remarks>
        /// Retorna tanto slots disponibles como ocupados para que el profesional
        /// o la secretaria puedan ver la agenda completa.
        /// </remarks>
        [HttpGet("professional/{professionalId}/range")]
        [ProducesResponseType(typeof(ResponseApi<List<AppointmentSlotResponseDto>>), 200)]
        public async Task<IActionResult> GetByProfessionalAndDateRange(
            Guid professionalId,
            [FromQuery] DateOnly dateFrom,
            [FromQuery] DateOnly dateTo)
        {
            var response = new ResponseApi<List<AppointmentSlotResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetByProfessionalAndDateRangeAsync(professionalId, dateFrom, dateTo);
                response.Message = "Slots obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot GetByProfessionalAndDateRange", ex, new { professionalId, dateFrom, dateTo });
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene todos los slots del sistema
        /// </summary>
        /// <returns>Lista completa de slots</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        /// <remarks>
        /// Endpoint administrativo. Usar con precaución ya que puede retornar grandes volúmenes de datos.
        /// Se recomienda usar el endpoint de búsqueda con filtros en su lugar.
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<List<AppointmentSlotResponseDto>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<AppointmentSlotResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetAllAsync();
                response.Message = "Slots obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot GetAll", ex);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Crea un slot de turno manualmente
        /// </summary>
        /// <param name="dto">Datos del slot a crear</param>
        /// <returns>Slot creado</returns>
        /// <response code="201">Slot creado exitosamente</response>
        /// <response code="400">Datos inválidos o slot duplicado</response>
        /// <remarks>
        /// Permite crear slots individuales fuera del proceso automático de generación.
        /// Útil para agregar horarios extras o cubrir necesidades puntuales.
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/appointmentslot
        ///     {
        ///         "professionalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "date": "2025-10-15",
        ///         "startTime": "14:00",
        ///         "endTime": "14:30",
        ///         "durationMins": 30
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<AppointmentSlotResponseDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentSlotDto dto)
        {
            var response = new ResponseApi<AppointmentSlotResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);

                response.IsSuccess = true;
                response.Data = created;
                response.Message = "Slot creado exitosamente";

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError("AppointmentSlot Create - Validation", ex, dto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot Create", ex, dto);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Elimina un slot de turno
        /// </summary>
        /// <param name="id">ID del slot a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Slot eliminado exitosamente</response>
        /// <response code="404">Slot no encontrado</response>
        /// <response code="400">No se puede eliminar un slot ocupado</response>
        /// <remarks>
        /// Solo se pueden eliminar slots disponibles (no ocupados).
        /// Si un slot tiene una cita asignada, primero se debe cancelar la cita.
        /// </remarks>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.DeleteAsync(id);
                response.Message = "Slot eliminado exitosamente";
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot Delete", ex, id);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Busca slots con filtros avanzados y paginación
        /// </summary>
        /// <param name="filter">Criterios de búsqueda</param>
        /// <returns>Resultado paginado de slots</returns>
        /// <response code="200">Búsqueda realizada exitosamente</response>
        /// <remarks>
        /// Permite filtrar por profesional, rango de fechas, disponibilidad y rango horario.
        /// Ideal para mostrar calendarios de disponibilidad y gestionar agendas.
        /// </remarks>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ResponseApi<PagedResult<AppointmentSlotResponseDto>>), 200)]
        public async Task<IActionResult> Search([FromQuery] AppointmentSlotFilterDto filter)
        {
            var response = new ResponseApi<PagedResult<AppointmentSlotResponseDto>>();

            try
            {
                var result = await _service.FindAsync(filter);

                response.IsSuccess = true;
                response.Data = result;
                response.Message = "Búsqueda exitosa";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot Search", ex, filter);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Genera slots masivamente para un profesional en un rango de fechas
        /// </summary>
        /// <param name="dto">Datos para generación de slots</param>
        /// <returns>Cantidad de slots generados</returns>
        /// <response code="200">Slots generados exitosamente</response>
        /// <response code="400">Datos inválidos o profesional sin horarios configurados</response>
        /// <remarks>
        /// Genera slots automáticamente basándose en:
        /// - Los horarios semanales configurados del profesional
        /// - Excluyendo días no laborales del profesional
        /// - Excluyendo feriados nacionales
        /// 
        /// Este proceso puede tardar según el rango de fechas.
        /// 
        /// Ejemplo de request:
        /// 
        ///     POST /api/appointmentslot/generate
        ///     {
        ///         "professionalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "dateFrom": "2025-10-01",
        ///         "dateTo": "2025-12-31"
        ///     }
        /// 
        /// </remarks>
        [HttpPost("generate")]
        [Authorize(Roles = "Admin,ScheduleManager")]
        [ProducesResponseType(typeof(ResponseApi<int>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GenerateSlots([FromBody] GenerateSlotsDto dto)
        {
            var response = new ResponseApi<int>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var slotsGenerated = await _service.GenerateSlotsAsync(dto);

                response.IsSuccess = true;
                response.Data = slotsGenerated;
                response.Message = $"{slotsGenerated} slots generados exitosamente";

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError("AppointmentSlot GenerateSlots - Validation", ex, dto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot GenerateSlots", ex, dto);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Reserva un slot asignándole una cita
        /// </summary>
        /// <param name="slotId">ID del slot a reservar</param>
        /// <param name="appointmentId">ID de la cita que ocupará el slot</param>
        /// <returns>Confirmación de reserva</returns>
        /// <response code="200">Slot reservado exitosamente</response>
        /// <response code="404">Slot no encontrado</response>
        /// <response code="400">Slot no disponible</response>
        /// <remarks>
        /// Este endpoint es llamado automáticamente cuando se crea una cita.
        /// Marca el slot como no disponible y lo vincula con la cita.
        /// </remarks>
        [HttpPost("{slotId}/book/{appointmentId}")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> BookSlot(Guid slotId, Guid appointmentId)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.BookSlotAsync(slotId, appointmentId);
                response.Message = "Slot reservado exitosamente";
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return NotFound(response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot BookSlot", ex, new { slotId, appointmentId });
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Libera un slot para que vuelva a estar disponible
        /// </summary>
        /// <param name="slotId">ID del slot a liberar</param>
        /// <returns>Confirmación de liberación</returns>
        /// <response code="200">Slot liberado exitosamente</response>
        /// <response code="404">Slot no encontrado</response>
        /// <remarks>
        /// Este endpoint es llamado automáticamente cuando se cancela una cita.
        /// Marca el slot como disponible nuevamente y elimina la vinculación con la cita.
        /// </remarks>
        [HttpPost("{slotId}/release")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ReleaseSlot(Guid slotId)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.ReleaseSlotAsync(slotId);
                response.Message = "Slot liberado exitosamente";
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
                _logger.LogError("AppointmentSlot ReleaseSlot", ex, slotId);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Elimina slots antiguos que ya pasaron
        /// </summary>
        /// <param name="beforeDate">Eliminar slots anteriores a esta fecha</param>
        /// <returns>Cantidad de slots eliminados</returns>
        /// <response code="200">Slots eliminados exitosamente</response>
        /// <remarks>
        /// Proceso de limpieza para eliminar slots viejos y disponibles.
        /// Solo elimina slots disponibles (no ocupados) para mantener el historial de citas.
        /// Se recomienda ejecutar como tarea programada mensualmente.
        /// 
        /// Ejemplo: DELETE /api/appointmentslot/cleanup?beforeDate=2025-09-01
        /// </remarks>
        [HttpDelete("cleanup")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseApi<int>), 200)]
        public async Task<IActionResult> DeleteOldSlots([FromQuery] DateOnly beforeDate)
        {
            var response = new ResponseApi<int>();
            try
            {
                var deletedCount = await _service.DeleteOldSlotsAsync(beforeDate);

                response.IsSuccess = true;
                response.Data = deletedCount;
                response.Message = $"{deletedCount} slots antiguos eliminados";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("AppointmentSlot DeleteOldSlots", ex, beforeDate);
                return BadRequest(response);
            }
        }
    }
}
