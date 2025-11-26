FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary
COPY SharedLibrary/SharedLibrary.csproj SharedLibrary/
COPY SharedLibrary/ SharedLibrary/

# Copy BlazorAdmin
COPY BlazorAdmin/BlazorAdmin.csproj BlazorAdmin/
RUN dotnet restore "BlazorAdmin/BlazorAdmin.csproj"

# Copy everything
COPY BlazorAdmin/ BlazorAdmin/
RUN dotnet build "BlazorAdmin/BlazorAdmin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorAdmin/BlazorAdmin.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorAdmin.dll"]
