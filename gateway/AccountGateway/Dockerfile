FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["gateway/AccountGateway/AccountGateway.csproj", "gateway/AccountGateway/"]
COPY ["shared/Core/Core.csproj", "shared/Core/"]
COPY ["shared/RabbitMQClient/RabbitMQClient.csproj", "shared/RabbitMQClient/"]
COPY ["shared/Application/Application.csproj", "shared/Application/"]
RUN dotnet restore "gateway/AccountGateway/AccountGateway.csproj"
COPY . .
WORKDIR "/src/gateway/AccountGateway"
RUN dotnet build "./AccountGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AccountGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AccountGateway.dll"]
