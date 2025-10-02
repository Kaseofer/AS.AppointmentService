using AS.AppointmentService.Application.Services;
using AS.AppointmentService.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;


namespace AS.AppointmentService.Application.IOC
{
    public static class AddApplicationServices
    {
        public static  IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAppointmentStatusService, AppointmentStatusService>();
            services.AddScoped<IAppointmentReasonService, AppointmentReasonService>();





            return services;
        }
    }
}
