using AS.AppointmentService.API.IOC;
using AS.AppointmentService.Application.IOC;
using AS.AppointmentService.Application.Mappers;
using AS.AppointmentService.Infrastructure.IOC;
using AS.AppointmentService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AgendaSaludDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention());


builder.Services.AddPresentationLayerService(builder.Configuration);
builder.Services.AddInfrastructureLayer();
builder.Services.AddApplicationLayer();

// Registrás los validadores que tengas en tu proyecto
builder.Services.AddFluentValidationAutoValidation();


// Configuraciones de servicios de tu arquitectura limpia
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});



builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfiles).Assembly);


var app = builder.Build();

// Habilitar Swagger solo si hay una variable de entorno específica
var enableSwagger = Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true"
                    || app.Environment.IsDevelopment();

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();