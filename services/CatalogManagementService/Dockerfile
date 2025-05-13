FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/CatalogManagementService/CatalogManagementService.csproj", "services/CatalogManagementService/"]
COPY ["shared/RabbitMQClient/RabbitMQClient.csproj", "shared/RabbitMQClient/"]
COPY ["shared/Core/Core.csproj", "shared/Core/"]
COPY ["shared/Application/Application.csproj", "shared/Application/"]
RUN dotnet restore "services/CatalogManagementService/CatalogManagementService.csproj"
COPY . .
WORKDIR "/src/services/CatalogManagementService"
RUN dotnet build "./CatalogManagementService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CatalogManagementService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogManagementService.dll"]
