using AS.AppointmentService.Application.Services;
using AS.AppointmentService.Application.Services.Intefaces;
using Microsoft.Extensions.DependencyInjection;


namespace AS.AppointmentService.Application.IOC
{
    public static class AddApplicationServices
    {
        public static  IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            // Pacientes
            services.AddScoped<IAgendaCitasService, AgendaCitasService>();
            services.AddScoped<IEstadoCitaService, EstadoCitaService>();
            services.AddScoped<IMotivoCitaService, MotivoCitaService>();





            return services;
        }
    }
}
