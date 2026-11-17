# Sofra — Agent Instructions

## What this is

Sofra is a **multi-vendor restaurant ordering marketplace**: customers order from approved restaurants, restaurant owners manage their own storefronts/menus/orders, administrators approve and police restaurants. Requirements live in `docs/PRD.md` and `docs/SRS.md` — the business rules **BR-01…BR-11** in the SRS are normative.

Stack: **.NET 10** — a versioned Web API (`Sofra.Api`) with Scalar/OpenAPI docs, following **Clean Architecture**. There is no MVC view layer.

## Layers & dependency rule

| Project | Responsibility | May reference |
|---|---|---|
| `Sofra.Domain` | Rich domain model: entities, enums, `DomainException`. No framework dependencies except ASP.NET Identity (for `AppUser`/`AppRole`) | — |
| `Sofra.Application` | CQRS features (MediatR + FluentValidation), `Result<T>`, `Error`, pagination, Mapster, `ICurrentUser` interface | Domain |
| `Sofra.Infrastructure` | EF Core (SQL Server) persistence, Identity stores, audit interceptor, per-entity configurations, migrations | Application, Domain |
| `Sofra.Api` (folder `src/Sofra.Presentation`) | Versioned REST controllers, error envelope (`Helper/`), exception handler (`Middleware/`), `ICurrentUser` implementation (`Services/`); **composition root** (`Program.cs`) | Infrastructure, Application |

Arrows point inward only. Never add EF/HTTP/`DbContext` to Domain or Application.

## Current state (2026-11-17)

- **Domain: complete** — 13 entities, 10 enums, rich model (factories, private setters, `DomainException` guards). **61 classical behavior tests, all green** (`test/Sofra.DomainTests`).
- **Infrastructure: EF model fully configured** — one `IEntityTypeConfiguration<T>` per entity in `Persistent/Configurations/` (`AppUser`/`AppRole` stay Identity-managed). Money `decimal(18,2)`, delete behaviors protect BR-04, `FoodCategory.Name` unique.
- **API: infrastructure done** — unified error envelope (`Helper/ApiErrorResponseFactory` + `Helper/ErrorExtensions.ToActionResult`), global exception handling (`Middleware/GlobalExceptionHandler`, `IExceptionHandler`), `ICurrentUser` implemented + registered. **No controllers exist yet.**
- **Application: `Features/` is empty** — no CQRS feature written yet. Open decision before the first handler: how handlers reach the database (Application has no reference to Infrastructure — e.g. an `IAppDbContext` abstraction; ask the user).
- **Migration is stale** — `20261117083850_Init` covers Identity tables only; all business entities need a migration (or full re-init) before the app can run.
- Serilog is package-referenced but not wired into `Program.cs`.

## Working agreement

1. **Plan first.** For any non-trivial change, present the plan (tables/lists of decisions and files to touch) and wait for approval before writing code.
2. **The user dictates architecture verbatim.** When the user names a type, pattern, or package, use it exactly — no renaming, restructuring, or "better" substitutes without asking.
3. **Skills.** Project skills live in `.agents/skills/` (flat files, not auto-loaded — this list is the index). **Read the matching one before working in that layer:**
   - `creating-domain-entities.md` — entity recipe, weak entities, BR enforcement map
   - `adding-cqrs-features.md` — MediatR 15 layout, Result/Error conventions, `ICurrentUser` usage
   - `api-endpoints.md` — error envelope, `ToActionResult`, exception handler, controller rules
   - `working-with-the-database.md` — configurations, migration workflow, stale-migration warning
   - `writing-domain-tests.md` — classical behavior tests, FluentAssertions, plain-English names, DRY helpers
4. SRS business rules are the source of truth for domain behavior; when code and doc conflict, surface it — don't silently pick a side.

## Commands

- Build: `dotnet build` — must pass before claiming anything "works".
- Tests: `dotnet test` — behavior suite in `test/Sofra.DomainTests`; must stay green.
- Run: `dotnet run --project src/Sofra.Presentation` → `https://localhost:7265`; docs (Development) at `/scalar/v1`, `/openapi/v1.json`.
- DB: `Server=localhost;Database=Sofra;Trusted_Connection=true;`. Migrations currently stale — see `working-with-the-database.md` for the re-init sequence before running the app.
