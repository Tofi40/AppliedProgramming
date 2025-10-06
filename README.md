# RoomieMatch Monorepo

This repository contains a simplified scaffold for the RoomieMatch platform, including a .NET 9 backend and placeholder structures for the Angular frontend, infrastructure, and documentation assets.

## Repository Structure

- `backend/`: ASP.NET Core Web API, EF Core models, matching engine, seed data, and controllers.
- `frontend/`: Placeholder for the Angular 20 application.
- `infra/`: Infrastructure-as-code, including Docker Compose.
- `docs/`: Documentation assets such as Postman collections.

## Prerequisites

- [.NET SDK 9.0-preview](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) or newer.
- PostgreSQL 14+ running locally or available via connection string.
- (Optional) Node.js 20+ and Angular CLI if you intend to work on the frontend placeholder.

## Backend Quickstart

1. Navigate to the API project:

   ```bash
   cd backend/RoomieMatch.Api
   ```

2. Restore packages and build the solution:

   ```bash
   dotnet restore
   dotnet build
   ```

3. Ensure the PostgreSQL connection string is configured. By default the API looks for the `ConnectionStrings:Postgres` setting in `appsettings.json`. You can override it with an environment variable:

   ```bash
   export ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=roomiematch;Username=postgres;Password=postgres"
   ```

4. Run database migrations and start the API:

   ```bash
   dotnet run
   ```

   On startup the application automatically applies EF Core migrations and seeds sample data.

5. Visit Swagger UI at `https://localhost:5001/swagger` (or the HTTP port shown in the console) to explore the available endpoints.

## Next Steps

- Flesh out the Angular frontend scaffold under `frontend/`.
- Add automated tests and CI pipelines to cover the matching engine and API surface.
- Containerize the solution using the infrastructure assets under `infra/` once they are implemented.
