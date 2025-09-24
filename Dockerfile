FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar todo el código
COPY . .

# Restaurar el proyecto API
RUN dotnet restore "AS.AppointmentService.Api\AS.AppointmentService.Api.csproj"

# Publicar la aplicación
RUN dotnet publish "AS.AppointmentService.Api\AS.AppointmentService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# ✅ CAMBIAR ESTA LÍNEA
ENTRYPOINT ["dotnet", "AS.AppointmentService.Api.dll"]