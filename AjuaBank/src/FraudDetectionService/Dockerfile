FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5300

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
COPY SharedLibrary/ SharedLibrary/

# Copy FraudDetectionService
COPY FraudDetectionService/FraudDetectionService.csproj FraudDetectionService/
RUN dotnet restore "FraudDetectionService/FraudDetectionService.csproj"

# Copy everything
COPY FraudDetectionService/ FraudDetectionService/
RUN dotnet build "FraudDetectionService/FraudDetectionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FraudDetectionService/FraudDetectionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FraudDetectionService.dll"]
