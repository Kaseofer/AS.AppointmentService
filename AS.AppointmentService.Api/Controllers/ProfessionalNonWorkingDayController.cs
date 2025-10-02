/*
 * PROFESSIONAL NON-WORKING DAYS ENDPOINTS
 * 
 * GET    /api/professionalnonworkingday/{id}                    - Obtener día no laboral por ID
 * GET    /api/professionalnonworkingday/professional/{professionalId}  - Obtener todos los días no laborales de un profesional
 * GET    /api/professionalnonworkingday/professional/{professionalId}/range  - Obtener días no laborales en un rango de fechas
 * GET    /api/professionalnonworkingday                         - Obtener todos
 * GET    /api/professionalnonworkingday/search                  - Buscar con filtros
 * POST   /api/professionalnonworkingday                         - Crear día no laboral
 * PUT    /api/professionalnonworkingday/{id}                    - Actualizar
 * DELETE /api/professionalnonworkingday/{id}                    - Eliminar
 * GET    /api/professionalnonworkingday/check/{professionalId}/{date}  - Verificar si es día no laboral
 */

using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.ProfessionalNonWorkingDay;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalNonWorkingDayController : ControllerBase
    {
        private readonly IProfessionalNonWorkingDayService _service;
        private readonly IAppLogger<ProfessionalNonWorkingDayController> _logger;

        public ProfessionalNonWorkingDayController(
            IProfessionalNonWorkingDayService service,
            IAppLogger<ProfessionalNonWorkingDayController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseApi<ProfessionalNonWorkingDayResponseDto>();
            try
            {
                var nonWorkingDay = await _service.GetByIdAsync(id);

                if (nonWorkingDay == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró el día no laboral";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = nonWorkingDay;
                response.Message = "Día no laboral encontrado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalNonWorkingDay GetById", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet("professional/{professionalId}")]
        public async Task<IActionResult> GetByProfessionalId(Guid professionalId)
        {
            var response = new ResponseApi<List<ProfessionalNonWorkingDayResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetByProfessionalIdAsync(professionalId);
                response.Message = "Días no laborales obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalNonWorkingDay GetByProfessionalId", ex, professionalId);
                return BadRequest(response);
            }
        }

        [HttpGet("professional/{professionalId}/range")]
        public async Task<IActionResult> GetByDateRange(
            Guid professionalId,
            [FromQuery] DateOnly dateFrom,
            [FromQuery] DateOnly dateTo)
        {
            var response = new ResponseApi<List<ProfessionalNonWorkingDayResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetByDateRangeAsync(professionalId, dateFrom, dateTo);
                response.Message = "Días no laborales obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalNonWorkingDay GetByDateRange", ex, new { professionalId, dateFrom, dateTo });
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<ProfessionalNonWorkingDayResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.GetAllAsync();
                response.Message = "Días no laborales obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalNonWorkingDay GetAll", ex);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfessionalNonWorkingDayDto dto)
        {
            var response = new ResponseApi<ProfessionalNonWorkingDayResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Obtener el usuario actual del token (si está implementado)
                // var currentUserId = User.FindFirst("sub")?.Value;
                // Guid? createdBy = currentUserId != null ? Guid.Parse(currentUserId) : null;

                Guid? createdBy = null; // Por ahora null, implementar cuando tengas claims

                var created = await _service.CreateAsync(dto, createdBy);

                response.IsSuccess = true;
                response.Data = created;
                response.Message = "Día no laboral creado exitosamente";

                return CreatedAtAction(nameof(GetById), new { id = created.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError("ProfessionalNonWorkingDay Create - Validation", ex, dto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalNonWorkingDay Create", ex, dto);
                return BadRequest(response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProfessionalNonWorkingDayDto dto)
        {
            var response = new ResponseApi<ProfessionalNonWorkingDayResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);

                response.IsSuccess = true;
                response.Data = updated;
                response.Message = "Día no laboral actualizado exitosamente";
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
                _logger.LogError("ProfessionalNonWorkingDay Update", ex, dto);
                return BadRequest(response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.DeleteAsync(id);
                response.Message = "Día no laboral eliminado exitosamente";
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
                _logger.LogError("ProfessionalNonWorkingDay Delete", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProfessionalNonWorkingDayFilterDto filter)
        {
            var response = new ResponseApi<PagedResult<ProfessionalNonWorkingDayResponseDto>>();

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
                _logger.LogError("ProfessionalNonWorkingDay Search", ex, filter);
                return BadRequest(response);
            }
        }

        [HttpGet("check/{professionalId}/{date}")]
        public async Task<IActionResult> CheckNonWorkingDay(Guid professionalId, DateOnly date, [FromQuery] TimeOnly? time = null)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _service.IsNonWorkingDayAsync(professionalId, date, time);
                response.Message = response.Data
                    ? "Es día no laboral"
                    : "Es día laboral";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalNonWorkingDay Check", ex, new { professionalId, date, time });
                return BadRequest(response);
            }
        }
    }
}