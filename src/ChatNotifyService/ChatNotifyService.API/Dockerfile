FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ChatNotifyService/ChatNotifyService.ABS/ChatNotifyService.ABS.csproj", "src/ChatNotifyService/ChatNotifyService.ABS/"]
COPY ["src/ChatNotifyService/ChatNotifyService.BLL/ChatNotifyService.BLL.csproj", "src/ChatNotifyService/ChatNotifyService.BLL/"]
COPY ["src/ChatNotifyService/ChatNotifyService.DAL/ChatNotifyService.DAL.csproj", "src/ChatNotifyService/ChatNotifyService.DAL/"]
COPY ["src/ChatNotifyService/ChatNotifyService.API/ChatNotifyService.API.csproj", "src/ChatNotifyService/ChatNotifyService.API/"]
RUN dotnet restore "src/ChatNotifyService/ChatNotifyService.API/ChatNotifyService.API.csproj"
COPY . .
WORKDIR "/src/src/ChatNotifyService/ChatNotifyService.API"
RUN dotnet build "./ChatNotifyService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChatNotifyService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatNotifyService.API.dll"]
