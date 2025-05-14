FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["gateway/CatalogGateway/CatalogGateway.csproj", "gateway/CatalogGateway/"]
COPY ["shared/Application/Application.csproj", "shared/Application/"]
COPY ["shared/Core/Core.csproj", "shared/Core/"]
COPY ["shared/RabbitMQClient/RabbitMQClient.csproj", "shared/RabbitMQClient/"]
RUN dotnet restore "gateway/CatalogGateway/CatalogGateway.csproj"
COPY . .
WORKDIR "/src/gateway/CatalogGateway"
RUN dotnet build "./CatalogGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CatalogGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CatalogGateway.dll"]
