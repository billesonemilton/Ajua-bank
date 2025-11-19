# AjuaBank - Scaffold

This scaffold contains minimal projects for Ajua Bank:
- ApiGateway (minimal)
- AuthService (minimal)
- TransactionService (minimal)
- FraudDetectionService (with ML placeholder)
- SharedLibrary
- BlazorAdmin (Blazor Server pages)

## Requirements
- .NET 8 SDK
- Docker & Docker Compose (optional, for local dev)
- PostgreSQL (or use docker-compose)

## Quick start (local)
1. Navigate to a service (e.g. src/TransactionService) and run:
   dotnet restore
   dotnet run

## Docker (compose)
From the project root:
   docker compose up --build

## Notes
This scaffold is a starting point. Fill in models, EF Core migrations, security (JWT), and messaging (RabbitMQ) as needed.
