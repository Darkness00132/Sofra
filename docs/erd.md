# Sofra — Entity-Relationship Diagram (ERD)

**A picture of the database, in plain terms.** Everything is a `Guid` key (the only exceptions are the ASP.NET Identity infrastructure tables, which use integer or composite keys). The model below is derived from the EF Core migrations (`Init`, `AddMenuItemImages`, `AddRestaurantImage`).

Related: [backend-architecture](backend-architecture.md) · [PRD](PRD.md)

## 1. Conventions to know before reading

- **Two base types.** `Entity` is just an `Id`. `AuditableEntity : Entity` adds `CreatedAt`, `LastModifiedAt?`, `ModifiedBy?`. `ModifiedBy` is only a **bare Guid column, not a foreign key** — it records *who* last touched the row, it doesn't reference a user table.
- **Only four tables are audited** — FoodCategory, FoodType, FoodItem, Restaurant. Their audit columns are stamped automatically by the save-changes interceptor, not by application code.
- **Enums are stored as strings** (`nvarchar(32)`), by a single global rule.
- **Long text** (image URLs, identity hashes, claim values) maps to `nvarchar(max)`; **money** is always `decimal(18,2)`.
- **How deletes behave** on each relationship: **Cascade** (delete the child rows too), **SetNull** (keep the row, just unlink it), or **Restrict** (block the delete — used to protect a price snapshot or stop a delete from cascading twice). All 24 edges are listed in §4.
- **Orders, payments, and transactions are plain `Entity`** (not audited). An order's life isn't tracked on the `Order` row — it's a separate `OrderStatusHistory` row per change.

## 2. The diagram

```mermaid
erDiagram
    AppUser      ||--o{ Cart                 : owns
    Restaurant   |o--o{ Cart                 : holds
    Cart         ||--o{ CartItem             : contains
    FoodItem     ||--o{ CartItem             : referenced
    FoodCategory ||--o{ FoodType             : classified
    Restaurant   ||--o{ FoodType             : owns
    FoodType     ||--o{ FoodItem             : contains

    AppUser      ||--o{ Order                : places
    Restaurant   ||--o{ Order                : fulfils
    Order        ||--o{ OrderItem            : contains
    FoodItem     ||--o{ OrderItem            : priced
    Order        ||--o{ OrderStatusHistory   : history
    AppUser      |o--o{ OrderStatusHistory   : changed_by
    Order        ||--o{ Payment              : paid_via
    Order        |o--o{ Transaction          : ledger
    Payment      |o--o{ Transaction          : ledger
    Restaurant   |o--o{ Transaction          : ledger

    AppUser      ||--o{ PasswordResetOtp     : otp
    AppUser      ||--o{ RefreshToken         : refresh
    AppUser      ||--o{ Restaurant           : owns
    AppUser      ||--o{ RestaurantApplication : applies
    AppUser      |o--o{ RestaurantApplication : reviewed_by
    Restaurant   |o--o{ RestaurantApplication : approved_as

    AppUser      }o--o{ AppRole              : has_roles

    AppUser    { Guid Id PK, string Email, bool EmailConfirmed, bool LockoutEnabled }
    AppRole    { Guid Id PK, string Name }
    Cart       { Guid Id PK, Guid UserId FK, Guid RestaurantId FK }
    CartItem   { Guid Id PK, Guid CartId FK, Guid FoodItemId FK, int Quantity }
    FoodCategory { Guid Id PK, string Name, string Description }
    FoodType   { Guid Id PK, Guid FoodCategoryId FK, Guid RestaurantId FK, string Name }
    FoodItem   { Guid Id PK, Guid FoodTypeId FK, string Name, decimal Price, FoodAvailability Availability }
    Order      { Guid Id PK, Guid CustomerId FK, Guid RestaurantId FK, OrderStatus Status, decimal Total }
    OrderItem  { Guid Id PK, Guid OrderId FK, Guid FoodItemId FK, int Quantity, decimal Price }
    OrderStatusHistory { Guid Id PK, Guid OrderId FK, Guid ChangedByUserId FK, OrderStatus Status, string Reason }
    Payment    { Guid Id PK, Guid OrderId FK, decimal Amount, PaymentStatus Status }
    Transaction { Guid Id PK, Guid OrderId FK, Guid PaymentId FK, Guid RestaurantId FK, TransactionType Type, decimal Amount }
    PasswordResetOtp { Guid Id PK, Guid UserId FK, string CodeHash, DateTime ExpiresAt }
    RefreshToken { Guid Id PK, Guid UserId FK, string Token, DateTime ExpiresAt }
    Restaurant { Guid Id PK, Guid OwnerId FK, string Name, RestaurantStatus Status, bool IsAcceptingOrders }
    RestaurantApplication { Guid Id PK, Guid ApplicantId FK, Guid RestaurantId FK, Guid ReviewedByUserId FK, string RestaurantName, RestaurantStatus Status }
```

## 3. The tables, explained

**People & roles**

- **AppUser** (`AspNetUsers`) — one row per registered user (customer, restaurant owner, or admin). Notable columns: `Email`/`NormalizedEmail` (indexed), `EmailConfirmed` (an unconfirmed email can't log in), and the lockout flags `LockoutEnabled`/`LockoutEnd` (used by the admin *lock account* feature).
- **AppRole** (`AspNetRoles`) — the user's role; seeded with **Customer**, **Restaurant Owner**, **Administrator**. Users and roles link many-to-many through the Identity join table.

**Menu (the catalog)**

- **FoodCategory** *(audited)* — the platform's top-level categories (Burger, Pizza…). `Name` must be unique; optional `Description`.
- **FoodType** *(audited)* — a restaurant's *own* sub-type under a platform category — e.g. under "Burger" a restaurant's "Cheeseburger". Points at both a `FoodCategory` and a `Restaurant`.
- **FoodItem** *(audited)* — the actual dish. Carries the required `Name`, `Description`, and `Price`, an optional image, and `Availability` (Available/Unavailable).

**Cart**

- **Cart** — one cart per user. `RestaurantId` is **optional on purpose**: a cart has no restaurant until its first item assigns one.
- **CartItem** — a row of that cart: which `FoodItem` and how many. Adding the same item again increments `Quantity` rather than creating a duplicate row.

**Restaurant & its application**

- **Restaurant** *(audited)* — the live storefront: `OwnerId` (the owner's user), `Name`/`Address`/`Phone`/`Description`, `Status` (pending/approved/rejected/suspended), `IsAcceptingOrders` (owner toggle), and an optional cover image.
- **RestaurantApplication** — the pre-approval submission. Has `ApplicantId`, the proposed storefront fields, `Status`, and an optional `RejectionReason`. Its `RestaurantId` stays empty until the admin **approves** it, at which point a `Restaurant` is created and linked. `ReviewedByUserId` records which admin made the call.

**Ordering**

- **Order** — the money row: who ordered (`CustomerId`), from whom (`RestaurantId`), fulfilment (Pickup/Delivery), payment method (Cash/Online), and the **price snapshot** — Subtotal, Tax, DeliveryFee, Discount, Total, all frozen at the moment of ordering (BR-11). Plus `DeliveryStatus`, `DeliveryAddress`, and `CreatedAt`/`CompletedAt`.
- **OrderItem** — a line on the order: the item, quantity, and the **unit price as it was at purchase time** (BR-04). The link to the menu item is *Restrict*, so a menu item can't be deleted while a historical order still references it.
- **OrderStatusHistory** — one immutable row per state change (who changed it, to which status, with what reason, when). This is how an order's full life is reconstructed.
- **Payment** — how the order was paid: `Amount`, `PaymentMethod`, `Status` (pending/completed/failed/refunded/cancelled), optional gateway `Metadata` and `PaidAt`.
- **Transaction** — a denormalized **ledger** entry. One row may point at any combination of `Order`, `Payment`, and `Restaurant` (all optional), and carries a `Type` (Debit/Credit), `Amount`, a reference, a `Status`, and `CreatedAt`. It's the audit trail for any money movement on the platform.

**Auth & security**

- **RefreshToken** — a user's single-use refresh token: `Token` (must be unique), `ExpiresAt`, and `RevokedAt` when rotated or logged out.
- **PasswordResetOtp** — the password-reset code, stored only as a `CodeHash`, with `ExpiresAt` and a `UsedAt` once consumed.
- **(Identity infrastructure)** — five standard tables (`AspNetUserRoles`, `AspNetUserClaims`, `AspNetRoleClaims`, `AspNetUserLogins`, `AspNetUserTokens`), all with cascade links back to AppUser/AppRole. They're managed by Identity.

## 4. Relationships and how deletes behave

`Child.Fk → Parent` · cardinality · delete rule · required?

| # | Edge | Card. | Delete | Req. |
|---|---|---|---|---|
| 1 | `Cart.UserId → AppUser` | 1:N | Cascade | yes |
| 2 | `Cart.RestaurantId → Restaurant` | 1:N | SetNull | **no** |
| 3 | `CartItem.CartId → Cart` | 1:N | Cascade | yes |
| 4 | `CartItem.FoodItemId → FoodItem` | 1:N | Restrict | yes |
| 5 | `FoodType.FoodCategoryId → FoodCategory` | 1:N | Restrict | yes |
| 6 | `FoodType.RestaurantId → Restaurant` | 1:N | Cascade | yes |
| 7 | `FoodItem.FoodTypeId → FoodType` | 1:N | Cascade | yes |
| 8 | `Order.CustomerId → AppUser` | 1:N | Restrict | yes |
| 9 | `Order.RestaurantId → Restaurant` | 1:N | Restrict | yes |
| 10 | `OrderItem.OrderId → Order` | 1:N | Cascade | yes |
| 11 | `OrderItem.FoodItemId → FoodItem` | 1:N | Restrict | yes |
| 12 | `OrderStatusHistory.OrderId → Order` | 1:N | Cascade | yes |
| 13 | `OrderStatusHistory.ChangedByUserId → AppUser` | 1:N | Restrict | **no** |
| 14 | `Payment.OrderId → Order` | 1:N | Restrict | yes |
| 15 | `PasswordResetOtp.UserId → AppUser` | 1:N | Cascade | yes |
| 16 | `RefreshToken.UserId → AppUser` | 1:N | Cascade | yes |
| 17 | `Restaurant.OwnerId → AppUser` | 1:N | Restrict | yes |
| 18 | `RestaurantApplication.ApplicantId → AppUser` | 1:N | Restrict | yes |
| 19 | `RestaurantApplication.ReviewedByUserId → AppUser` | 1:N | Restrict | **no** |
| 20 | `RestaurantApplication.RestaurantId → Restaurant` | 1:N | SetNull | **no** |
| 21 | `Transaction.OrderId → Order` | 1:N | SetNull | **no** |
| 22 | `Transaction.PaymentId → Payment` | 1:N | SetNull | **no** |
| 23 | `Transaction.RestaurantId → Restaurant` | 1:N | SetNull | **no** |
| 24 | `AppUser ↔ AppRole` (via join) | N:M | Cascade | yes |

**How to read the delete rules:**

- **Cascade** on owned children (cart items, order lines, status rows, tokens, the menu chain under a food category) — take the child out when the parent goes.
- **SetNull** on the optional links — a cart without its restaurant, a transaction whose order was removed, an application whose restaurant is gone: keep the row, just lose the link.
- **Restrict** guards the history: an order line's `FoodItem`, an order's customer/restaurant, and a payment's order can't have their targets deleted while the row depends on them. `Payment.OrderId` is deliberately Restrict because SQL Server won't allow the double-cascade path `Order → Payment → Transaction` to coexist with the direct `Order → Transaction` edge.
- The optional application links (`RestaurantId`, `ReviewedByUserId`) and the whole `Transaction` row use **SetNull**.

## 5. All the enum values

| Enum | Values |
|---|---|
| `OrderStatus` | `Pending`, `Accepted`, `Preparing`, `Ready`, `Completed`, `Rejected`, `Cancelled` |
| `DeliveryStatus` | `NotApplicable`, `Pending`, `Collected`, `InTransit`, `Delivered` |
| `FulfilmentType` | `Pickup`, `Delivery` |
| `PaymentMethod` | `CashOnDelivery`, `OnlinePayment` |
| `PaymentStatus` | `Pending`, `Completed`, `Failed`, `Refunded`, `Cancelled` |
| `TransactionType` | `Debit`, `Credit` |
| `TransactionStatus` | `Pending`, `Completed`, `Failed`, `Reversed` |
| `RestaurantStatus` | `PendingApproval`, `Approved`, `Rejected`, `Suspended` |
| `FoodAvailability` | `Available`, `Unavailable` |
| `OrderCancellationReason` | `CustomerRequested`, `RestaurantCannotFulfil`, `Expired` *(not a column — it's a parameter of `Order.Cancel`, written into the history `Reason`)* |
