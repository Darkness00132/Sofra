# Sofra — Project Instructions

## What this is

Sofra is a **multi-vendor restaurant ordering marketplace**: customers order from multiple approved restaurants, restaurant owners manage their own storefronts/menus/orders, administrators approve and police restaurants. Requirements live in `docs/PRD.md` and `docs/SRS.md` (business rules BR-01…BR-11 are normative).

The stack is **ASP.NET Core 10** — a versioned Web API (the original MVC view layer was removed; the Presentation project is `Sofra.Api.csproj`), following **Clean Architecture**.

## Layers & dependency rule

| Project | Responsibility | May reference |
|---|---|---|
| `Sofra.Domain` | Rich domain model: entities, enums, `DomainException`. No framework dependencies except ASP.NET Identity (for `AppUser`/`AppRole`) | — |
| `Sofra.Application` | CQRS features (MediatR + FluentValidation), `Result<T>`, `Error`, pagination, Mapster | Domain |
| `Sofra.Infrastructure` | EF Core (SQL Server) persistence, Identity stores, audit interceptor, migrations | Application, Domain |
| `Sofra.Api` (folder `Sofra.Presentation`) | Versioned REST controllers, error envelope (`Helper/`), exception handler (`Middleware/`), `ICurrentUser` implementation (`Services/`); **composition root** (`Program.cs`) | Infrastructure, Application |

Arrows point inward only. Never add EF/HTTP/`DbContext` to Domain or Application.

## Current state (2026-11-17)

- **Domain: complete.** 13 entities, 10 enums, rich domain model (factories, private setters, `DomainException` guards).
- **Infrastructure: EF model fully configured** — `IEntityTypeConfiguration<T>` per entity in `Persistent/Configurations/` (12 files; `AppUser`/`AppRole` stay Identity-managed). Money `decimal(18,2)`, delete behaviors protect BR-04, `FoodCategory.Name` unique.
- **Tests:** `test/Sofra.DomainTests` (xUnit) — classical behavior tests for critical domain rules; conventions in `.agents/skills/writing-domain-tests.md`.
- **Application:** `Common/` (Result, Error, PagedResult, PaginationRequest) and the MediatR `ValidationBehavior` are done. `Features/` is **empty** — no CQRS feature exists yet.
- **API:** error envelope done (`Helper/ApiErrorResponseFactory` + `Helper/ErrorExtensions.ToActionResult` + `Middleware/GlobalExceptionHandler` via `IExceptionHandler`), `ICurrentUser` interface (Application) + `Services/CurrentUser` implementation registered in DI. No controllers exist yet. **Build has not been verified** (shell was broken during this work — run `dotnet build`).
- **Migration is stale:** `20261117083850_Init` and the model snapshot contain **only the Identity tables**. All business entities are in `SofraDbContext`/the model but were never migrated. A new migration must be added (or the DB recreated) before the app can reach business tables.
- Serilog is package-referenced in `Sofra.Api.csproj` but **not wired into `Program.cs`** yet.
- No unit/integration test projects yet.

## Working agreement

1. **Plan first.** For any non-trivial change, present the plan (tables/lists of decisions and files to touch) and wait for approval before writing code.
2. **The user dictates architecture verbatim.** When the user names a type, pattern, or package, use it exactly — do not rename, restructure, or substitute "a better" alternative without asking.
3. **Skills.** Project skills live in `.agents/skills/` as flat files: `creating-domain-entities.md`, `adding-cqrs-features.md`, `api-endpoints.md`, `working-with-the-database.md`. **Read the matching skill before working in that layer** — it encodes the conventions already established in the code. (Note: flat `.md` files are not auto-loaded by ZCode; this list is the index — pick the file for the layer you're touching.)
4. SRS business rules (BR-01…11) in `docs/SRS.md` are the source of truth for domain behaviour; when code and doc conflict, surface it — don't silently pick a side.

## Commands

- Build: `dotnet build` (from repo root) — must pass before claiming anything "works".
- Tests: `dotnet test` (behavior suite lives in `test/Sofra.DomainTests`).
- Run: `dotnet run --project src/Sofra.Presentation` → `https://localhost:7265`.
- API docs (Development only): Scalar UI at `/scalar/v1`, OpenAPI at `/openapi/v1.json`.
- DB: `Server=localhost;Database=Sofra;Trusted_Connection=true;` (EF tools already referenced; `dotnet-ef` CLI is available).
