FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/EventScheduleService/EventScheduleService.ABS/EventScheduleService.ABS.csproj", "src/EventScheduleService/EventScheduleService.ABS/"]
COPY ["src/EventScheduleService/EventScheduleService.BLL/EventScheduleService.BLL.csproj", "src/EventScheduleService/EventScheduleService.BLL/"]
COPY ["src/EventScheduleService/EventScheduleService.DAL/EventScheduleService.DAL.csproj", "src/EventScheduleService/EventScheduleService.DAL/"]
COPY ["src/EventScheduleService/EventScheduleService.API/EventScheduleService.API.csproj", "src/EventScheduleService/EventScheduleService.API/"]
RUN dotnet restore "src/EventScheduleService/EventScheduleService.API/EventScheduleService.API.csproj"
COPY . .
WORKDIR "/src/src/EventScheduleService/EventScheduleService.API"
RUN dotnet build "./EventScheduleService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./EventScheduleService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventScheduleService.API.dll"]
