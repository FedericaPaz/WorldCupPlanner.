# WorldCupPlanner

A full-stack web application to plan, manage and simulate a World Cup–style football tournament, from the group stage to the final.

Built with **C# / .NET 8**, **Blazor Server**, **Entity Framework Core** and **SQL Server**, following a layered architecture and backed by **650+ unit tests**.

> Academic project developed for *Software Design 1* (Diseño de Aplicaciones 1) at Universidad ORT Uruguay, 2026.

## Features

- **Tournament management:** create and edit teams, stadiums and matches.
- **Fixture generation:** automatically build the group stage and track group standings and rankings.
- **Match simulation:** simulate results with two interchangeable engines, purely random or probabilistic.
- **Knockout stage:** calculate knockout-round matchups based on group results.
- **Users and roles:** registration, login and password change, with three roles (System Administrator, Editor and Journalist) and role-based access.
- **Import and export:** import data from CSV files and export fixtures and logs to CSV or Excel (.xlsx).
- **Notifications and activity log:** keep users informed and record actions performed in the system.

## Tech stack

| Area | Technologies |
|---|---|
| Language and runtime | C#, .NET 8 |
| Frontend | Blazor Server, Razor components, Bootstrap, CSS |
| Backend | ASP.NET Core, layered services, DTOs |
| Data access | Entity Framework Core 8, SQL Server (Azure SQL Edge in Docker), EF migrations |
| Testing | MSTest, EF Core InMemory provider, hand-written stubs, Coverlet |
| Other | ClosedXML (Excel export), Blazored.LocalStorage, Docker Compose |

## Architecture

The solution is split into projects by responsibility, so each layer depends only on abstractions of the layer below:

```
WorldCupPlanner/
├── Web/            Blazor Server UI (pages, components, layout)
├── IServices/      Service interfaces and page facades
├── Services/       Use cases: fixture, simulation, knockout stage, import/export, auth, users, logs
├── DTOs/           Data transfer objects between UI and services
├── Domain/         Entities, enums, builders, standing/ranking calculators and policies
├── IRepository/    Repository interfaces
├── Repository/     EF Core DbContext, repositories and migrations
├── DomainTests/    Unit tests for the domain
├── ServicesTests/  Unit tests for services (with stubs)
└── RepositoryTests/ Unit tests for repositories (EF Core InMemory)
```

Design highlights:

- **SOLID and GRASP principles**, with small, segregated interfaces (for example, separate query and edit services).
- **Factory pattern** to select the simulation engine at runtime (`SimulationEngineFactory`).
- **Facade pattern** to keep Blazor pages thin (`PageFacades`).
- **Builder and calculators** in the domain for fixture creation, standings and rankings.
- **Dependency injection** throughout, which allows services and repositories to be tested in isolation.

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (to run SQL Server locally)
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### 1. Start the database

From the `WorldCupPlanner` folder:

```bash
docker compose up -d
```

This starts a SQL Server–compatible container (Azure SQL Edge) on port `1433`. The connection string is in `Web/appsettings.json`.

### 2. Apply the migrations

```bash
dotnet ef database update --project Repository --startup-project Web
```

### 3. Run the application

```bash
dotnet run --project Web
```

Open the URL shown in the console (for example `http://localhost:5175`).

### 4. Run the tests

```bash
dotnet test WorldCupPlanner.sln
```

## Testing

The project includes **650+ unit tests** written with MSTest:

- **Domain tests** cover entities, validations and calculators.
- **Service tests** use hand-written stubs for repositories and dependencies (including a fake clock), so business logic is tested without a database.
- **Repository tests** run against the EF Core InMemory provider.

## Authors

Developed by a team of three students at Universidad ORT Uruguay:

- Federica Paz Vitale – [GitHub](https://github.com/YOUR-USERNAME) · [LinkedIn](https://linkedin.com/in/federica-paz-vitale)
- Teammate name
- Teammate name