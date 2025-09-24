using AS.AppointmentService.Application.Dtos;
using AS.AppointmentService.Core.Entities;
using AutoMapper;

namespace AS.AppointmentService.Application.Mappers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {


            // Turno
            CreateMap<AgendaCitas, AgendaCitasDto>().ReverseMap();



            // Motivo y Estado
            CreateMap<MotivoCita, MotivoCitaDto>().ReverseMap();
            CreateMap<EstadoCita, EstadoCitaDto>().ReverseMap();

        }
    }

}
