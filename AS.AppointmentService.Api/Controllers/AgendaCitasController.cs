using AgendaSaludApp.Application.Common;
using AS.AppointmentService.Api.Common;
using AS.AppointmentService.Application.Dtos;
using AS.AppointmentService.Application.Services.Intefaces;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Mvc;

namespace AS.AppointmentService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AgendaCitasController : ControllerBase
    {
        protected readonly IAgendaCitasService _agendaCitasService;
        protected readonly IAppLogger<AgendaCitasController> _logger;

        public AgendaCitasController(IAgendaCitasService agendaCitasService, IAppLogger<AgendaCitasController> logger)
        {
            _agendaCitasService = agendaCitasService;
            _logger = logger;
        }


        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = new ResponseApi<AgendaCitasDto>();

            try
            {

                response.Data = await _agendaCitasService.GetByIdAsync(id);
                response.IsSuccess = true;
                response.Message = "Profesional encontrado";

                return Ok(response);

            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(e);

                _logger.LogError("Agenda Citas GetID", e);

                return BadRequest(response);
            }
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<AgendaCitasDto>>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _agendaCitasService.GetAllAsync();
                response.Message = "Consulta Exitosa";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(e);
                _logger.LogError("Agenda Citas GetAll",e);

                return BadRequest(response);
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] AgendaCitasDto agendaCitasDto)
        {
            var response = new ResponseApi<AgendaCitasDto>();

            try
            {
                response.IsSuccess = true;
                response.Data = await _agendaCitasService.CreateAsync(agendaCitasDto); ;
                response.Message = "Agenda de cita creada exitosamente";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(e);

                _logger.LogError("Agenda Citas Create",e ,agendaCitasDto);

                return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] AgendaCitasDto agendaCitasDto)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _agendaCitasService.UpdateAsync(agendaCitasDto); ;
                response.Message = "Agenda de cita actualizada exitosamente";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(e);

                _logger.LogError("Agenda Citas Update",e, agendaCitasDto);

                return BadRequest(response);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new ResponseApi<bool>();
            try
            {
                response.IsSuccess = true;
                response.Data = await _agendaCitasService.DeleteAsync(id);
                response.Message = "Agenda de cita eliminada exitosamente";
                return Ok(response);
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(e);

                _logger.LogError("Agenda Citas Delete", e, id);
                return BadRequest(response);
            }
        }


    }

}
