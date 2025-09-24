using AS.AppointmentService.Application.IOC;
using AS.AppointmentService.Application.Mappers;
using AS.AppointmentService.Infrastructure.IOC;
using AS.AppointmentService.Infrastructure.Logger;
using AS.AppointmentService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AgendaSaludDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AgendaSalud_Appointments_Db"))
    .UseSnakeCaseNamingConvention());




//Logger
builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(FileLogger<>));

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Para Railway - usar puerto dinámico/*
/*var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");
*/

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();