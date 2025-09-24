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
    public class MotivoCitaController : ControllerBase
    {
        private readonly IMotivoCitaService _motivoCitaService;
        private readonly IAppLogger<MotivoCitaController> _logger;
        public MotivoCitaController(IMotivoCitaService motivoCitaService, IAppLogger<MotivoCitaController> logger)
        {
            _motivoCitaService = motivoCitaService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var response = new ResponseApi<MotivoCitaDto>();
            try
            {
                var motivo = await _motivoCitaService.GetByIdAsync(id);
                if (motivo == null)
                {
                    response.IsSuccess = false;
                    response.Message = "No se encontró el motivo";
                    return NotFound(response);
                }
                response.IsSuccess = true;
                response.Data = motivo;
                response.Message = "Motivo encontrado";

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);

                _logger.LogError("MotivoCita GetID:", ex, id);

                return BadRequest(response);
            }
            
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAll()
        {
            var response = new ResponseApi<List<MotivoCitaDto>>();
            try
            {
                var motivos = await _motivoCitaService.GetAllAsync();
                response.IsSuccess = true;
                response.Data = motivos;
                response.Message = "Motivos obtenidos con éxito";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ExceptionHelper.GetFullMessage(ex);
                _logger.LogError("MotivoCita GetAll:", ex);

                return BadRequest(response);
            }
        }
    }
}
