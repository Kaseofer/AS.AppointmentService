FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solo archivos csproj necesarios
COPY ["AS.AppointmentService.Api/*.csproj", "AS.AppointmentService.Api/"]
COPY ["AS.AppointmentService.Application/*.csproj", "AS.AppointmentService.Application/"]
COPY ["AS.AppointmentService.Core/*.csproj", "AS.AppointmentService.Core/"]
COPY ["AS.AppointmentService.Infrastructure/*.csproj", "AS.AppointmentService.Infrastructure/"]

# Restore
RUN dotnet restore "AS.AppointmentService.Api/AS.AppointmentService.Api.csproj"

# Copia todo el código
COPY . .

# Build y publish SOLO del proyecto Api
WORKDIR "/src/AS.AppointmentService.Api"
RUN dotnet build "AS.AppointmentService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AS.AppointmentService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AS.AppointmentService.Api.dll"]