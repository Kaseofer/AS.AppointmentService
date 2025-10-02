using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Common;
using AS.AppointmentService.Application.Dtos.Professional;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/*
GET /api/professionalschedule/{id} - Por ID
GET /api/professionalschedule/professional/{professionalId} - Por profesional
GET /api/professionalschedule - Todos
GET /api/professionalschedule/search - Búsqueda con filtros
POST /api/professionalschedule - Crear
PUT /api/professionalschedule/{id} - Actualizar
DELETE /api/professionalschedule/{id} - Eliminar
 */


namespace AS.AppointmentService.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProfessionalScheduleController : ControllerBase
    {
        private readonly IProfessionalScheduleService _scheduleService;
        private readonly IAppLogger<ProfessionalScheduleController> _logger;

        public ProfessionalScheduleController(
            IProfessionalScheduleService scheduleService,
            IAppLogger<ProfessionalScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = new ResponseApi<ProfessionalScheduleResponseDto>();
            try
            {
                var schedule = await _scheduleService.GetByIdAsync(id);

                if (schedule == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró el horario";
                    return NotFound(response);
                }

                response.IsSuccess = true;
                response.Data = schedule;
                response.Message = "Horario encontrado";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalSchedule GetById", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet("professional/{professionalId}")]
        public async Task<IActionResult> GetByProfessionalId(Guid professionalId)
        {
            var response = new ResponseApi<List<ProfessionalScheduleResponseDto>>();
            try
            {
                var schedules = await _scheduleService.GetByProfessionalIdAsync(professionalId);

                response.IsSuccess = true;
                response.Data = schedules;
                response.Message = "Horarios obtenidos exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalSchedule GetByProfessionalId", ex, professionalId);
                return BadRequest(response);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<ProfessionalScheduleResponseDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _scheduleService.GetAllAsync();
                response.Message = "Lista de horarios obtenida exitosamente";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalSchedule GetAll", ex);
                return BadRequest(response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfessionalScheduleDto scheduleDto)
        {
            var response = new ResponseApi<ProfessionalScheduleResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var scheduleCreated = await _scheduleService.CreateAsync(scheduleDto);

                response.IsSuccess = true;
                response.Data = scheduleCreated;
                response.Message = "Horario creado exitosamente";

                return CreatedAtAction(nameof(GetById), new { id = scheduleCreated.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                _logger.LogError("ProfessionalSchedule Create - Validation", ex, scheduleDto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalSchedule Create", ex, scheduleDto);
                return BadRequest(response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProfessionalScheduleDto scheduleDto)
        {
            var response = new ResponseApi<ProfessionalScheduleResponseDto>();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var scheduleUpdated = await _scheduleService.UpdateAsync(id, scheduleDto);

                response.IsSuccess = true;
                response.Data = scheduleUpdated;
                response.Message = "Horario actualizado exitosamente";
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
                _logger.LogError("ProfessionalSchedule Update - Validation", ex, scheduleDto);
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalSchedule Update", ex, scheduleDto);
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
                response.Data = await _scheduleService.DeleteAsync(id);
                response.Message = "Horario eliminado exitosamente";
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
                _logger.LogError("ProfessionalSchedule Delete", ex, id);
                return BadRequest(response);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProfessionalScheduleFilterDto filter)
        {
            var response = new ResponseApi<PagedResult<ProfessionalScheduleResponseDto>>();

            try
            {
                var result = await _scheduleService.FindAsync(filter);

                response.IsSuccess = true;
                response.Data = result;
                response.Message = "Búsqueda exitosa";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("ProfessionalSchedule Search", ex, filter);
                return BadRequest(response);
            }
        }
    }
}