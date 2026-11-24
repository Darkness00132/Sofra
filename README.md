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
src/frontend          React 19 + TypeScript + Vite SPA (npm project, not part of the .NET solution)
docs/                 PRD (normative business rules BR-01…BR-11), architecture docs, ERD, API reference
test/                 Sofra.DomainTests — domain behavior suite
```

## Getting started

**Prerequisites:** .NET 10 SDK, a local SQL Server instance.

```bash
git clone <repo-url> && cd Sofra
dotnet build
dotnet test          # domain behavior suite
dotnet run --project src/Sofra.Api
```

- API: `https://localhost:7265` (also `http://localhost:5183`)
- Docs (Development only): Scalar UI at `/scalar/v1`, OpenAPI at `/openapi/v1.json`
- Frontend: `npm run dev` in `src/frontend/` → `http://localhost:5173` (proxies `/api` to the API)
- Connection string: `appsettings.json` → `DefaultConnection` (defaults to `localhost` with trusted connection)
- Migrations apply automatically at startup (hosted service); the admin account is seeded on first run.

## Documentation

- [`docs/PRD.md`](docs/PRD.md) — product requirements, business rules (BR-01…BR-11), functional requirements, open decisions
- [`docs/backend-architecture.md`](docs/backend-architecture.md) — backend architecture (projects, layers, CQRS, API conventions)
- [`docs/frontend-architecture.md`](docs/frontend-architecture.md) — frontend architecture (React, feature slices, shared layer)
- [`docs/erd.md`](docs/erd.md) — entity-relationship diagram of the database
- [`docs/API.md`](docs/API.md) — endpoint reference

## Status

MVP v1 complete and behavior-tested: domain model, all CQRS features and controllers, persistence with migrations, and the frontend. Several business decisions remain open (revenue model, delivery responsibility, guest checkout, cancellation policy — see PRD §13).
