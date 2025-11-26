FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5200

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
COPY SharedLibrary/ SharedLibrary/

# Copy TransactionService
COPY TransactionService/TransactionService.csproj TransactionService/
RUN dotnet restore "TransactionService/TransactionService.csproj"

# Copy everything
COPY TransactionService/ TransactionService/
RUN dotnet build "TransactionService/TransactionService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionService/TransactionService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionService.dll"]
