FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RGamaFelix.TimeTracker.EntryPoint/RGamaFelix.TimeTracker.EntryPoint.csproj", "RGamaFelix.TimeTracker.EntryPoint/"]
RUN dotnet restore "RGamaFelix.TimeTracker.EntryPoint/RGamaFelix.TimeTracker.EntryPoint.csproj"
COPY . .
WORKDIR "/src/RGamaFelix.TimeTracker.EntryPoint"
RUN dotnet build "RGamaFelix.TimeTracker.EntryPoint.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RGamaFelix.TimeTracker.EntryPoint.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RGamaFelix.TimeTracker.EntryPoint.dll"]
