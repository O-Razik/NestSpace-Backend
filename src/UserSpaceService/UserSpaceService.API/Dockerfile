FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/UserSpaceService/UserSpaceService.ABS/UserSpaceService.ABS.csproj", "src/UserSpaceService/UserSpaceService.ABS/"]
COPY ["src/UserSpaceService/UserSpaceService.DAL/UserSpaceService.DAL.csproj", "src/UserSpaceService/UserSpaceService.DAL/"]
COPY ["src/UserSpaceService/UserSpaceService.BLL/UserSpaceService.BLL.csproj", "src/UserSpaceService/UserSpaceService.BLL/"]
COPY ["src/UserSpaceService/UserSpaceService.API/UserSpaceService.API.csproj", "src/UserSpaceService/UserSpaceService.API/"]
RUN dotnet restore "src/UserSpaceService/UserSpaceService.API/UserSpaceService.API.csproj"
COPY . .
WORKDIR "/src/src/UserSpaceService/UserSpaceService.API"
RUN dotnet build "./UserSpaceService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UserSpaceService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserSpaceService.API.dll"]
