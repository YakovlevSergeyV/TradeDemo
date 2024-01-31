#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Microservices/Services/TradesReader/TradesReader.Api/TradesReader.Api.csproj", "src/Microservices/Services/TradesReader/TradesReader.Api/"]
COPY ["src/Microservices/Common/Microservices.Common/Microservices.Common.csproj", "src/Microservices/Common/Microservices.Common/"]
COPY ["src/Common/Infrastructure.Common/Infrastructure.Common.csproj", "src/Common/Infrastructure.Common/"]
COPY ["src/Common/Guardly/Guardly.csproj", "src/Common/Guardly/"]
COPY ["src/Common/ServiceWorker/ServiceWorker.csproj", "src/Common/ServiceWorker/"]
COPY ["src/Microservices/Event/EventBusRabbitMQ/EventBusRabbitMQ.csproj", "src/Microservices/Event/EventBusRabbitMQ/"]
COPY ["src/Microservices/Event/EventBus/EventBus.csproj", "src/Microservices/Event/EventBus/"]
COPY ["src/Microservices/Common/Microservices.Logging/Microservices.Logging.csproj", "src/Microservices/Common/Microservices.Logging/"]
COPY ["src/Microservices/Common/Resilience.Http/Resilience.Http.csproj", "src/Microservices/Common/Resilience.Http/"]
RUN dotnet restore "src/Microservices/Services/TradesReader/TradesReader.Api/TradesReader.Api.csproj"
COPY . .
WORKDIR "/src/src/Microservices/Services/TradesReader/TradesReader.Api"
RUN dotnet build "TradesReader.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TradesReader.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TradesReader.Api.dll"]