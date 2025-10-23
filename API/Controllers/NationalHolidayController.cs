using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.NationalHoliday;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    /// <summary>
    /// Gestión de feriados nacionales
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NationalHolidayController : ControllerBase
    {
        private readonly INationalHolidayService _service;
        private readonly IAppLogger<NationalHolidayController> _logger;

        public NationalHolidayController(
            INationalHolidayService service,
            IAppLogger<NationalHolidayController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene un feriado por su ID
        /// </summary>
        /// <param name="id">ID del feriado</param>
        /// <returns>Información del feriado</returns>
        /// <response code="200">Feriado encontrado</response>
        /// <response code="404">Feriado no encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseApi<NationalHolidayResponseDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseApi<NationalHolidayResponseDto>();
            try
            {
                var holiday = await _service.GetByIdAsync(id);

                if (holiday == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró el feriado";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = holiday;
                response.Message = "Feriado encontrado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("NationalHoliday GetById", ex, id);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene el feriado de una fecha específica
        /// </summary>
        /// <param name="date">Fecha a consultar (formato: YYYY-MM-DD)</param>
        /// <returns>Información del feriado si existe</returns>
        /// <response code="200">Consulta exitosa (puede retornar null si no es feriado)</response>
        [HttpGet("date/{date}")]
        [ProducesResponseType(typeof(ResponseApi<NationalHolidayResponseDto>), 200)]
        public async Task<IActionResult> GetByDate(DateOnly date)
        {
            var response = new ResponseApi<NationalHolidayResponseDto>();
            try
            {
                var holiday = await _service.GetByDateAsync(date);

                response.IsSuccess = true;
                response.Data = holiday;
                response.Message = holiday != null ? "Es feriado" : "No es feriado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("NationalHoliday GetByDate", ex, date);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene todos los feriados de un año específico
        /// </summary>
        /// <param name="year">Año a consultar</param>
        /// <returns>Lista de feriados del año</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        [HttpGet("year/{year}")]
        [ProducesResponseType(typeof(ResponseApi<List<NationalHolidayResponseDto>>), 200)]
        public async Task<IActionResult> GetByYear(int year)
        {
            var response = new ResponseApi<List<NationalHolidayResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetByYearAsync(year);
                response.Message = "Feriados del año obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("NationalHoliday GetByYear", ex, year);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene todos los feriados registrados
        /// </summary>
        /// <returns>Lista completa de feriados</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        [HttpGet]
        [ProducesResponseType(typeof(ResponseApi<List<NationalHolidayResponseDto>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<NationalHolidayResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetAllAsync();
                response.Message = "Feriados obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("NationalHoliday GetAll", ex);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Obtiene solo los feriados activos
        /// </summary>
        /// <returns>Lista de feriados activos ordenados por fecha</returns>
        /// <response code="200">Lista obtenida exitosamente</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(ResponseApi<List<NationalHolidayResponseDto>>), 200)]
        public async Task<IActionResult> GetActive()
        {
            var response = new ResponseApi<List<NationalHolidayResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetActiveAsync();
                response.Message = "Feriados activos obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("NationalHoliday GetActive", ex);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Crea un nuevo feriado nacional
        /// </summary>
        /// <param name="dto">Datos del feriado a crear</param>
        /// <returns>Feriado creado</returns>
        /// <response code="201">Feriado creado exitosamente</response>
        /// <response code="400">Datos inválidos o fecha duplicada</response>
        /// <response code="401">No autorizado</response>
        /// <remarks>
        /// Ejemplo de request:
        /// 
        ///     POST /api/nationalholiday
        ///     {
        ///         "date": "2025-12-25",
        ///         "name": "Navidad",
        ///         "isRecurring": true,
        ///         "active": true
        ///     }
        /// 
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseApi<NationalHolidayResponseDto>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Create([FromBody] CreateNationalHolidayDto dto)
        {
            var response = new ResponseApi<NationalHolidayResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);

                response.IsSuccess = true;
                response.Data = created;
                response.Message = "Feriado creado exitosamente";

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError("NationalHoliday Create - Validation", ex, dto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("NationalHoliday Create", ex, dto);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Actualiza un feriado existente
        /// </summary>
        /// <param name="id">ID del feriado a actualizar</param>
        /// <param name="dto">Datos a actualizar</param>
        /// <returns>Feriado actualizado</returns>
        /// <response code="200">Feriado actualizado exitosamente</response>
        /// <response code="404">Feriado no encontrado</response>
        /// <response code="401">No autorizado</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseApi<NationalHolidayResponseDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNationalHolidayDto dto)
        {
            var response = new ResponseApi<NationalHolidayResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                response.IsSuccess = true;
                response.Data = updated;
                response.Message = "Feriado actualizado exitosamente";
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
                _logger.LogError("NationalHoliday Update", ex, dto);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Elimina un feriado
        /// </summary>
        /// <param name="id">ID del feriado a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        /// <response code="200">Feriado eliminado exitosamente</response>
        /// <response code="404">Feriado no encontrado</response>
        /// <response code="401">No autorizado</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.DeleteAsync(id);
                response.Message = "Feriado eliminado exitosamente";
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
                _logger.LogError("NationalHoliday Delete", ex, id);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Busca feriados con filtros avanzados
        /// </summary>
        /// <param name="filter">Filtros de búsqueda (año, mes, rango de fechas, etc)</param>
        /// <returns>Resultado paginado de feriados</returns>
        /// <response code="200">Búsqueda realizada exitosamente</response>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ResponseApi<PagedResult<NationalHolidayResponseDto>>), 200)]
        public async Task<IActionResult> Search([FromQuery] NationalHolidayFilterDto filter)
        {
            var response = new ResponseApi<PagedResult<NationalHolidayResponseDto>>();

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
                _logger.LogError("NationalHoliday Search", ex, filter);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Verifica si una fecha es feriado nacional
        /// </summary>
        /// <param name="date">Fecha a verificar (formato: YYYY-MM-DD)</param>
        /// <returns>True si es feriado, False si no lo es</returns>
        /// <response code="200">Verificación exitosa</response>
        [HttpGet("check/{date}")]
        [ProducesResponseType(typeof(ResponseApi<bool>), 200)]
        public async Task<IActionResult> CheckHoliday(DateOnly date)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.IsHolidayAsync(date);
                response.Message = response.Data
                    ? "Es feriado nacional"
                    : "No es feriado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("NationalHoliday Check", ex, date);
                return BadRequest(response);
            }
        }
    }
}