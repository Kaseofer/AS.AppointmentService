using AS.AppointmentService.Application.Settings;
using AS.AppointmentService.Infrastructure.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace AS.AppointmentService.API.IOC
{
    public static class DependencyInjectionExtensions
    {


        public static IServiceCollection AddPresentationLayerService(
                                                          this IServiceCollection services,
                                                                IConfiguration configuration)
        {



            //Logger
            services.AddSingleton(typeof(IAppLogger<>), typeof(FileLogger<>));

            Console.WriteLine("Adding JWT settings...");
            services.Configure<JwtSettings>(options =>
            {
                configuration.GetSection("Jwt").Bind(options);
                options.Key = Environment.GetEnvironmentVariable("Jwt__Key") ?? options.Key;
            });

            // COMENTAR TEMPORALMENTE TODOS LOS HEALTH CHECKS
            Console.WriteLine("Adding health checks...");
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("AuthService is running"));

            // COMENTAR TODO ESTO TEMPORALMENTE:
            // .AddDbContextCheck<AuthenticationDbContext>("database")
            // .AddCheck("jwt-configuration", ...)
            // .AddCheck("google-oauth", ...)
            // .AddCheck("memory", ...)

            Console.WriteLine("Adding basic services...");
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // COMENTAR CORS TEMPORALMENTE
            // Console.WriteLine("Adding CORS...");
            Console.WriteLine("Adding CORS...");
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            Console.WriteLine("Adding logger...");
            services.AddSingleton(typeof(IAppLogger<>), typeof(FileLogger<>));

            Console.WriteLine("Adding authentication...");
            Console.WriteLine("Adding authentication...");

            Console.WriteLine("Adding JWT authentication...");
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 var key = Environment.GetEnvironmentVariable("Jwt__Key") ?? configuration["Jwt:Key"];
                 var issuer = configuration["Jwt:Issuer"];
                 var audience = configuration["Jwt:Audience"];

                 Console.WriteLine("========================================");
                 Console.WriteLine($"[JWT] Key: {key?.Substring(0, Math.Min(10, key?.Length ?? 0))}... (length: {key?.Length})");
                 Console.WriteLine($"[JWT] Issuer: '{issuer}'");
                 Console.WriteLine($"[JWT] Audience: '{audience}'");
                 Console.WriteLine("========================================");

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                     ValidateIssuer = true,
                     ValidIssuer = issuer,
                     ValidateAudience = true,
                     ValidAudience = audience,
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.Zero
                 };

                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                         Console.WriteLine($"[JWT] Token received: {token?.Substring(0, Math.Min(50, token?.Length ?? 0))}...");
                         return Task.CompletedTask;
                     },
                     OnAuthenticationFailed = context =>
                     {
                         Console.WriteLine($"❌ [JWT] Auth FAILED: {context.Exception.GetType().Name}");
                         Console.WriteLine($"❌ [JWT] Message: {context.Exception.Message}");

                         if (context.Exception.InnerException != null)
                         {
                             Console.WriteLine($"❌ [JWT] Inner: {context.Exception.InnerException.Message}");
                         }

                         return Task.CompletedTask;
                     },
                     OnTokenValidated = context =>
                     {
                         Console.WriteLine("✅ [JWT] Token VALIDATED successfully!");
                         return Task.CompletedTask;
                     }
                 };
             });


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Appointment Service API",
                    Version = "v1",
                    Description = "API para gestión de turnos y citas médicas"
                });

                // Incluir comentarios XML
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Opcional: Incluir XML de la capa Application si tiene DTOs documentados
                var applicationXml = "AS.AppointmentService.Application.xml";
                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXml);
                if (File.Exists(applicationXmlPath))
                {
                    c.IncludeXmlComments(applicationXmlPath);
                }

                // Configurar JWT en Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
            });

            // NO agregues políticas que requieran autenticación por defecto
            services.AddAuthorization(); // Sin políticas por defecto

            return services;
        }
    }
}