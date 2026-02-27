# См. статью по ссылке https://aka.ms/customizecontainer, чтобы узнать как настроить контейнер отладки и как Visual Studio использует этот Dockerfile для создания образов для ускорения отладки.

# Этот этап используется при запуске из VS в быстром режиме (по умолчанию для конфигурации отладки)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Этот этап используется для сборки проекта службы
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PatientDemo.WebApi/PatientDemo.WebApi.csproj", "PatientDemo.WebApi/"]
COPY ["PatientDemo.Application/PatientDemo.Application.csproj", "PatientDemo.Application/"]
COPY ["PatientDemo.Domain/PatientDemo.Domain.csproj", "PatientDemo.Domain/"]
COPY ["PatientDemo.Shared/PatientDemo.Shared.csproj", "PatientDemo.Shared/"]
COPY ["PatientDemo.Persistence/PatientDemo.Persistence.csproj", "PatientDemo.Persistence/"]
RUN dotnet restore "./PatientDemo.WebApi/PatientDemo.WebApi.csproj"
COPY . .
WORKDIR "/src/PatientDemo.WebApi"
RUN dotnet build "./PatientDemo.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Этот этап используется для публикации проекта службы, который будет скопирован на последний этап
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./PatientDemo.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Этот этап используется в рабочей среде или при запуске из VS в обычном режиме (по умолчанию, когда конфигурация отладки не используется)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PatientDemo.WebApi.dll"]