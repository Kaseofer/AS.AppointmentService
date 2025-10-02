using AS.AppointmentService.Application.Dtos.Appointment;
using AS.AppointmentService.Core.Entities;
using AutoMapper;

namespace AS.AppointmentService.Application.Mappers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // ========== APPOINTMENT ==========
            // Para CREATE
            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.IsBooked, opt => opt.Ignore())
                .ForMember(dest => dest.IsExpired, opt => opt.Ignore())
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Reason, opt => opt.Ignore());

            // Para UPDATE
            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.IsBooked, opt => opt.Ignore())
                .ForMember(dest => dest.IsExpired, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Reason, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Para RESPONSE
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason));

            // ========== APPOINTMENT STATUS ==========
            CreateMap<AppointmentStatus, AppointmentStatusResponseDto>();

            // ========== APPOINTMENT REASON ==========
            CreateMap<AppointmentReason, AppointmentReasonResponseDto>();
        }
    }
}