FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/UtilityBills.Host/UtilityBills.Host.csproj", "src/UtilityBills.Host/"]
COPY ["src/UtilityBills.Application/UtilityBills.Application.csproj", "src/UtilityBills.Application/"]
COPY ["src/UtilityBills/UtilityBills.csproj", "src/UtilityBills/"]
COPY ["src/UtilityBills.Aspire.AppHost/UtilityBills.Aspire.AppHost.ServiceDefaults/UtilityBills.Aspire.AppHost.ServiceDefaults.csproj", "src/UtilityBills.Aspire.AppHost/UtilityBills.Aspire.AppHost.ServiceDefaults/"]
COPY ["src/UtilityBills.Infrastructure/UtilityBills.Infrastructure.csproj", "src/UtilityBills.Infrastructure/"]
COPY ["src/UtilityBills.Telegram/UtilityBills.Telegram.csproj", "src/UtilityBills.Telegram/"]
RUN dotnet restore "src/UtilityBills.Host/UtilityBills.Host.csproj"
COPY . .
WORKDIR "/src/src/UtilityBills.Host"
RUN dotnet build "UtilityBills.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "UtilityBills.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UtilityBills.Host.dll"]
