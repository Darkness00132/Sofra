# Sofra — Backend Architecture

**What this is.** A .NET 10 Web API built with clean architecture: four projects in `src/` that talk strictly inward.

```
Sofra.Domain  ←  Sofra.Application  ←  Sofra.Infrastructure  ←  Sofra.Api
```

The rule is mechanical: an inner project never references an outer one. `Domain` depends on no runtime library (only a MediatR reference for a domain-event marker that's unused for now). The framework-specific packages — EF Core, JWT, Gmail SMTP, Cloudinary, Scalar docs — live in the two outermost projects only, keeping the business logic portable and testable.

Related: [frontend-architecture](frontend-architecture.md) · [erd](erd.md) · [API](API.md) · [PRD](PRD.md)

## 1. The four projects

| Project | What it is | Depends on |
|---|---|---|
| **Sofra.Domain** | entities, enums, exceptions — the pure business model | nothing |
| **Sofra.Application** | CQRS feature slices: commands/queries, handlers, validation, mapping | Domain |
| **Sofra.Infrastructure** | EF Core + SQL Server, Identity, JWT, email, image storage | Application, Domain |
| **Sofra.Api** | the versioned HTTP edge — controllers, docs, error envelope | Application, Infrastructure |

## 2. Sofra.Domain — where the business rules live

The Domain is the heart of the system. Every entity is **rich**: a private parameterless constructor plus a static factory, and all properties are `private set`. You can't set a property directly — you call a method named for its intent — which is how the rules get enforced.

**Base types.** `Entity` is just a `Guid Id`. `AuditableEntity : Entity` adds `CreatedAt`, `LastModifiedAt?`, `ModifiedBy?` — `ModifiedBy` is a bare Guid column, **not a foreign key** (informational only). Only four aggregates are audited — `FoodCategory`, `FoodType`, `FoodItem`, `Restaurant` — and those columns are stamped by an interceptor in Infrastructure (§4.5), so they're meaningless outside the persistence boundary. The rest (`Order`, `Cart`, `Payment`, …) are plain `Entity` and stamp their own `CreatedAt` in their factories.

**Entities.** Sixteen, covering users/roles (ASP.NET Identity, Guid keys), the restaurant → food type → food item menu chain, ordering (`Order`, `OrderItem`, and `OrderStatusHistory` which records every state change), plus `Cart`, `Payment` and the `Transaction` ledger, `RefreshToken`, and `PasswordResetOtp`. Every field, type, and foreign key is catalogued in [erd.md](erd.md).

**Building and changing.** Every object is built with `Create(...)` (or `FromApprovedApplication` for a restaurant born from a review) and changed through intent-named mutators — `MakeAvailable`, `Suspend`, `Accept`, `Cancel`. `Order` is a real state machine: `Accept → StartPreparing → MarkReady → Complete`, alongside `Reject` and `Cancel`; each transition appends an `OrderStatusHistory` row. A cart holds one restaurant only — `AddItem` throws if you hand it an item from a different one.

**Guards throw `DomainException`.** Invalid transitions — accepting an already-cancelled order, mixing two restaurants in one cart — throw `DomainException` (plain `Exception`). The Application layer either lets it escape (→ HTTP 422 below) or catches it and turns it into a friendlier conflict (e.g. `CART_RESTAURANT_MISMATCH` → 409).

**Enums are strings, end to end.** All ten enums (order status, delivery status, payment method, …) are stored as `nvarchar(32)` in the database and serialized as JSON strings. One quirk: `OrderCancellationReason` is a parameter of `Order.Cancel`, not a column (it's recorded in the history `Reason` text).

## 3. Sofra.Application — CQRS feature slices

**The result contract.** Handlers never return raw DTOs — they return `Result` or `Result<T>`: either a `Response` or an `Error`. An `Error` is a SCREAMING_SNAKE code (e.g. `RESTAURANT_NOT_FOUND`), a description, and a type (`Validation`, `NotFound`, `Conflict`, `BusinessRule`, …). Controllers just check `IsFailure` and turn it into HTTP.

**Shared contracts** (`Common/Interfaces/`): a minimal `IRepository<T>` (with SQL-side paging), `IUnitOfWork` (save/transaction), `ICurrentUser` (who's calling — declared here, implemented in Api), the outbound ports for email/token/file-storage, `IRestaurantOwnershipGuard` (lets every owner-side handler resolve "my own restaurant" or a clean 404), one named repository interface per aggregate (12 total), plus pagination types, role constants, and the shared image-upload rules. Every request also passes a MediatR **open behavior** that runs its FluentValidation validators and turns field errors into a 400.

**Feature layout** is one folder per action — `Features/<Area>/<Action>/` with the command- or query-record, its handler, and its validator, plus shared `Dtos/` and `Mapping/` at the area level. The areas:

| Area | Actions | What it does |
|---|---|---|
| Authentication | 9 | login, register, refresh, logout, profile, password reset, email verification |
| Carts | 5 | read cart, add / update / remove / clear items |
| Discovery | 2 | public restaurant list + details |
| FoodCategories | 4 | manage the platform's category catalogue |
| Menus | 10 | owner-side menu types + items, availability, images |
| Orders | 11 | place, preview, list (three audiences), status, cancel, dashboards |
| Restaurants | 13 | owner apply/update/storefront/image + admin review & status |
| Admin | 6 | platform-wide accounts, orders, payments, dashboard, lock |

Conventions: reads and writes are separate records even when they share a handler; every list takes a filter record and returns a paged result; owner-side handlers start by asserting ownership. One `IRegister` mapping per area drives both the SQL projection (read) and in-memory `Adapt` (write) — see §4.4.

## 4. Sofra.Infrastructure — persistence and the outside world

**Context.** `SofraDbContext : IdentityDbContext<AppUser, AppRole, Guid>` — the four domain `DbSet`s plus Identity's. A global convention converts every enum to `nvarchar(32)`. Relations live in fourteen `IEntityTypeConfiguration`s: money is `decimal(18,2)` everywhere, unique indexes where needed (FoodCategory.Name, RefreshToken.Token), and delete behaviors chosen deliberately (Cascade for owned children, SetNull for optional links, **Restrict** to protect price snapshots and block double cascades) — full list in [erd.md §4](erd.md).

**Repositories push projection into SQL.** Instead of loading whole graphs and reshaping in memory, reads use Mapster.EFCore's `ProjectToType<TDto>` to materialize exactly the DTO shape the API needs, in SQL. The same `IRegister` configs in Application drive both the SQL projection on the read path and the in-memory `Adapt` on the write path — one config, two uses, which is what replaced all the earlier hand-written response mappers. Each aggregate has a dedicated repository (the order repo, for example, does the projected detail read, three paged summary lists, and the dashboard's single-round-trip aggregates; the cart repo keeps one full-graph read because its handler mutates the domain object). Filter clauses live in per-area `QueryableExtensions`.

**Auditing.** A `SaveChangesInterceptor` stamps `CreatedAt`/`LastModifiedAt` (from an injected `TimeProvider`) and `ModifiedBy` (the current user's claim from `HttpContext`) on the audited entities.

**Integration services** implement the Application's ports: `JwtTokenProvider` (15-minute access JWT + a single-use 7-day refresh row), `GmailEmailSender` (MailKit, SMTP 587), and `CloudinaryFileStorage` (deliberately rule-free — the image rules live in the Application validators).

**DI and startup** (`AddInfrastructure`): EF Core + interceptor, Identity (with the default token providers that email confirmation and password reset require), and three hosted services running in this order at startup: **apply migrations → seed roles** (Customer / Restaurant / Administrator) **→ seed the admin** (from config, defaults `admin@sofra.test`).

## 5. Sofra.Api — the HTTP edge

**Pipeline:** Serilog request logging → CORS (one policy, the Vite dev origin) → global exception handler → HTTPS redirect → authentication → authorization → controllers → docs. JSON is configured so enums are strings and every date carries a `Z` (matching the DB and the frontend). The API is URL-versioned (`/api/v1`); OpenAPI + Scalar docs are development-only.

**One failure shape, bare successes.** Every error — a `Result.Error`, a binding error, or an uncaught exception — exits through one factory producing a consistent problem-details body: status, title, a SCREAMING_SNAKE `code`, detail, traceId, and a per-field `errors` map on validation failures. Business-rule violations become **422**. Successes are the bare payload, no envelope.

**19 controllers, split by audience** (not by verb):

| Audience | Controllers (route prefix) |
|---|---|
| public | restaurant discovery, restaurant's public menu, food categories, auth (all `AllowAnonymous`) |
| customer | profile (`auth/me`), cart, orders (place / list / `preview` / `{id}/cancel`) |
| restaurant owner | applications, `restaurants/me`, `restaurants/me/menu`, owner orders, owner dashboard |
| admin | applications, restaurants, orders, accounts (incl. `{id}/lock`), payments, food categories, dashboard — all gated `[Authorize(Roles = Administrator)]` |

Every action is the same three lines: send to MediatR → on failure return the error envelope → otherwise `Ok(response)`. Endpoint-by-endpoint reference: [API.md](API.md).

## 6. What a request looks like, end to end

`POST /api/v1/cart/items { foodItemId, quantity }`:

1. The controller (authenticated) sends `AddCartItemCommand` into MediatR.
2. The open behavior runs the validator — any field failure becomes a 400 with an `errors` map.
3. The handler loads the cart (full graph, since it mutates it), calls `cart.AddItem(...)` inside try/catch, saves, and projects the fresh cart to its response DTO via the feature's `IRegister`.
4. A mixed-restaurant attempt throws `DomainException`, which the handler catches and turns into `CART_RESTAURANT_MISMATCH` → 409. Otherwise → `200` + the cart.

Reads follow the same shape minus the mutation: filter → SQL projection → paged result → bare payload.
