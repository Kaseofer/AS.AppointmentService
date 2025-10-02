
using AS.AppointmentService.Infrastructure.Persistence.Repositories;
using AS.AppointmentService.Infrastructure.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AS.AppointmentService.Infrastructure.IOC
{
    public static class AddInfrastructureServices
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services)
        {
            // Aquí puedes registrar tus servicios de infraestructura, como repositorios, contextos de base de datos, etc.
            // services.AddScoped<IYourRepository, YourRepositoryImplementation>();
            // Repositorios


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            return services;
        }
    }
}
