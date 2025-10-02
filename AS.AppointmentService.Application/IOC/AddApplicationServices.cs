using AS.AppointmentService.Application.Services;
using AS.AppointmentService.Application.Services.Interfaces;
using AS.UserManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace AS.AppointmentService.Application.IOC
{
    public static class AddApplicationServices
    {
        public static  IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<IAppointmentService, AppointmentService.Application.Services.AppointmentService>();
            services.AddScoped<IAppointmentStatusService, AppointmentStatusService>();
            services.AddScoped<IAppointmentReasonService, AppointmentReasonService>();
            services.AddScoped<IProfessionalNonWorkingDayService, ProfessionalNonWorkingDayService>();
            services.AddScoped<INationalHolidayService, NationalHolidayService>();
            services.AddScoped<ISlotGenerationConfigService, SlotGenerationConfigService>();




            return services;
        }
    }
}
