FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5100

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
COPY SharedLibrary/ SharedLibrary/

# Copy AuthService
COPY AuthService/AuthService.csproj AuthService/
RUN dotnet restore "AuthService/AuthService.csproj"

# Copy everything
COPY AuthService/ AuthService/
RUN dotnet build "AuthService/AuthService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService/AuthService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]
