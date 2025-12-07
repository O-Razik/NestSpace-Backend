FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NestSpace-Gateway/NestSpace-Gateway.csproj", "src/NestSpace-Gateway/"]
RUN dotnet restore "src/NestSpace-Gateway/NestSpace-Gateway.csproj"
COPY . .
WORKDIR "/src/src/NestSpace-Gateway"
RUN dotnet build "./NestSpace-Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./NestSpace-Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NestSpace-Gateway.dll"]
