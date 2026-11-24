# Sofra

Sofra is a **multi-vendor restaurant ordering marketplace**: customers discover restaurants, browse menus, and order food from different restaurants through one platform. Restaurant owners run their own storefronts — menus, availability, and incoming orders — while administrators approve restaurants and oversee the marketplace.

## Features (MVP)

- **Customers** — browse and search approved restaurants, explore menus, manage a single-restaurant cart, place and track orders, view order history.
- **Restaurant owners** — apply to join the platform, manage restaurant info, organize menus (platform category → restaurant menu type → food item), control item availability and order acceptance, accept/reject/prepare orders.
- **Administrators** — review and approve/reject restaurant applications, suspend or reinstate restaurants, inspect order records.

## Technology

| | |
|---|---|
| Runtime | .NET 10 — ASP.NET Core Web API (versioned, OpenAPI + Scalar docs) |
| Architecture | Clean Architecture — Domain / Application / Infrastructure / Api |
| Data | EF Core 10 + SQL Server, ASP.NET Core Identity (Guid keys), enums stored as strings |
| Application tier | MediatR 15 (CQRS), FluentValidation pipeline, Mapster, `Result<T>` pattern |
| Testing | xUnit + FluentAssertions — classical behavior tests for critical domain rules |

## Solution structure

```
Sofra.Domain          entities, enums, DomainException — rich domain model, no framework deps
Sofra.Application     CQRS features, Result/Error, pagination, ICurrentUser interface
Sofra.Infrastructure  EF Core (SQL Server), Identity, audit interceptor, entity configurations
Sofra.Api            the versioned REST API — controllers, error envelope, docs UI
docs/                 PRD and SRS (business rules BR-01…BR-11 are normative)
test/                 Sofra.DomainTests — domain behavior suite
```

## Getting started

**Prerequisites:** .NET 10 SDK, a local SQL Server instance.

```bash
git clone <repo-url> && cd Sofra
dotnet build
dotnet test          # 61 domain behavior tests
dotnet run --project src/Sofra.Api
```

- API: `https://localhost:7265`
- Docs (Development only): Scalar UI at `/scalar/v1`, OpenAPI at `/openapi/v1.json`
- Connection string: `appsettings.json` → `DefaultConnection` (defaults to `localhost` with trusted connection)

**Database note:** the current `Init` migration predates the domain model and only covers the Identity tables. Before running the app, regenerate migrations:

```bash
dotnet ef database drop --force
# delete src/Sofra.Infrastructure/Persistent/Migrations/*
dotnet ef migrations add Init --startup-project src/Sofra.Api
dotnet ef database update
```

## Documentation

- [`docs/PRD.md`](docs/PRD.md) — product requirements, MVP scope
- [`docs/SRS.md`](docs/SRS.md) — functional requirements, business rules (BR-01…BR-11), order lifecycle

## Status

MVP in progress: domain model complete and behavior-tested, persistence and API infrastructure in place. CQRS features and controllers are next. Several business decisions remain open (payment methods, guest checkout, cancellation policy — see SRS §8).
