# Sofra — Product Requirements Document (PRD)

**Version:** 3.0 — updated 2026-11-24: absorbs and supersedes `docs/SRS.md` (now deleted); all functional requirements, business rules, order lifecycle and non-functional requirements live here
**Related documents:** `docs/backend-architecture.md`, `docs/frontend-architecture.md`, `docs/erd.md` (database model), `docs/API.md` (endpoint reference)

## 1. Product Overview

**Sofra** is a multi-vendor restaurant platform that allows customers to discover restaurants, browse menus, and order food from different restaurants through one platform.

The platform gives restaurants their own storefronts and tools to manage menus and incoming orders, while administrators oversee the marketplace.

## 2. Problem Statement

Customers need a convenient way to discover restaurants and order food online. Restaurants need an accessible platform to showcase their menus and manage online orders without building their own ordering systems.

Sofra brings both sides together in one marketplace.

## 3. Product Vision

To create a simple and accessible platform that connects customers with restaurants and makes online food ordering convenient for everyone.

## 4. Target Users

- **Customers:** people who want to discover restaurants and order food online.
- **Restaurant Owners:** approved businesses that manage their own storefront, menus, and orders.
- **Platform Administrators:** people responsible for approving restaurants and overseeing the marketplace.

Each role can access only the operations permitted for that role.

## 5. Core Features

### Customer Experience

- Discover restaurants: browse, search by name, and filter.
- Explore menus: platform-level food categories (e.g. Burger, Pizza), each restaurant's own menu types under those categories, and food items with descriptions, prices, availability, and images.
- Shop with a cart that holds items from **one restaurant at a time**; adding from another restaurant requires clearing the cart first.
- Check out with a chosen fulfilment method — **pickup or delivery** (delivery requires an address) — and pay by **cash or online payment**.
- Review the complete order, including fees and total, before submitting.
- Place orders and watch them progress through the order lifecycle, including delivery tracking.
- Cancel orders while the applicable cancellation policy still allows it.
- Review previous orders with their original prices and details.

### Restaurant Management

- Join via an **application** reviewed by administrators; the restaurant storefront is created only after approval.
- Manage restaurant information (name, address, phone, description, image).
- Build the menu: create menu types under platform food categories and manage food items (name, description, price, availability, image).
- Toggle whether the restaurant is accepting orders.
- Receive incoming orders and decide: accept or reject.
- Process accepted orders through preparation and fulfilment until completion.
- View order history with original purchase details.

### Platform Administration

- Review restaurant applications; approve or reject (with a reason).
- Suspend or reinstate restaurants; a suspended restaurant cannot accept orders.
- Manage customer and restaurant accounts (including locking accounts).
- Manage platform-level food categories (create, update, remove, image).
- Inspect order records, payments, and the platform dashboard for operational oversight.

## 6. Ordering Model (decided)

The following product decisions are **resolved** and reflected in the platform:

| Decision | Resolution |
|---|---|
| Fulfilment methods | **Pickup and Delivery** — customer chooses at checkout; delivery requires an address |
| Payment methods | **Cash and Online payment** — chosen at checkout; every order has its own payment record |
| Order ownership | **One restaurant per order** (and one restaurant per cart) |
| Price integrity | Item prices are **locked at order time**; later menu changes never alter past orders |
| Order lifecycle | **Pending → Accepted → Preparing → Ready → Completed**, with **Rejected** and **Cancelled** as terminal outcomes |
| Delivery tracking | Delivery orders track their own delivery status (**Collected → Delivered**), separate from the order status |
| Payment recording | Every order payment moves through its own payment record (pending → completed/failed/refunded/cancelled) |
| Money movements | A transaction ledger records debits and credits tied to orders, payments, and restaurants |
| Cancellation reasons | Customer cancels with a reason (customer requested, changed mind, …); restaurants can reject with "cannot fulfil" |

## 7. Business Rules

These rules are **normative** — the domain model enforces them and code references them by number.

**BR-01 — Restaurant Approval:** Restaurants must be approved by an administrator before they can accept customer orders.

**BR-02 — Single-Restaurant Orders:** Every cart and every order belongs to exactly one restaurant.

**BR-03 — Restaurant Data Isolation:** Restaurant owners may only manage their own restaurant's information and orders (one owner account per restaurant; an owner cannot own a second restaurant or hold more than one application at a time).

**BR-04 — Price Integrity:** Menu price changes never alter the prices recorded in existing orders (order line items snapshot item name, price, and quantity at purchase time).

**BR-05 — Availability:** Unavailable food items cannot be added to a cart or ordered.

**BR-06 — Order Acceptance:** An order remains pending until the restaurant accepts or rejects it.

**BR-07 — Order Cancellation:** Cancellation is governed by the order's status and the defined cancellation policy; a cancelled or rejected order is irreversible.

**BR-08 — Order Completion:** Only permitted status transitions may occur, and an order cannot be completed before the required fulfilment steps.

**BR-09 — Restaurant Suspension:** A suspended restaurant cannot accept new orders or appear as ordering-eligible.

**BR-10 — Order Ownership:** Customers may access only their own orders, except for authorized administrative access.

**BR-11 — Total Calculation:** Order totals must accurately reflect the item prices, fees, and taxes confirmed at checkout; the preview and the placed order use the same pricing calculation.

## 8. Order Lifecycle

The standard order lifecycle:

1. **Pending** — the order has been submitted and awaits the restaurant's decision.
2. **Accepted** — the restaurant has agreed to fulfil the order.
3. **Preparing** — the restaurant is preparing the food.
4. **Ready** — the order is ready for fulfilment.
5. **Completed** — the order has been fulfilled.

Alternative terminal outcomes:

- **Rejected** — the restaurant declines the order.
- **Cancelled** — the order is cancelled under the applicable policy.

Delivery orders carry an additional, independent delivery status (**Collected → Delivered**) that tracks the courier hand-off and completion of delivery.

## 9. Functional Requirements

### 9.1 Account Management

- **FR-01:** Users can register (email, name, password); registration signs the user in and returns an email confirmation token.
- **FR-02:** Registered users can sign in and sign out; sign-in requires a confirmed email and issues an access token plus a refresh token.
- **FR-03:** Users can update permitted profile information (name, profile picture).
- **FR-04:** Protected operations are restricted to authenticated and authorized users (role claims: Customer, Restaurant Owner, Administrator).
- **FR-05:** Users can recover account access via OTP-based password reset (email link, single-use code).

### 9.2 Restaurant Discovery

- **FR-06:** The platform displays approved restaurants that are eligible to appear (not suspended).
- **FR-07:** Customers can view restaurant details, menu, prices, and availability.
- **FR-08:** Customers can search for restaurants by name.
- **FR-09:** Customers can filter restaurants (e.g. by food category).
- **FR-10:** When a restaurant is not accepting orders, this is clearly indicated.

### 9.3 Restaurant Management

- **FR-11:** Restaurant owners can submit applications to join Sofra (one pending application at a time).
- **FR-12:** Restaurant owners can update their own restaurant's information.
- **FR-13:** Restaurant owners can create, update, and remove menu types under platform food categories.
- **FR-14:** Restaurant owners can create, update, and remove food items, including images.
- **FR-15:** Food items carry a name, description, price, availability, and optional image.
- **FR-16:** Restaurant owners can mark food items available or unavailable.
- **FR-17:** Restaurant owners can control whether their restaurant is accepting orders.
- **FR-18:** Restaurant owners can only manage their own restaurant's information and orders (BR-03).

### 9.4 Shopping Cart

- **FR-19:** Customers can add food items to their cart.
- **FR-20:** Customers can change item quantities and remove items before checkout.
- **FR-21:** The cart contains items from only one restaurant at a time (BR-02).
- **FR-22:** Adding an item from another restaurant requires the existing cart to be cleared first.
- **FR-23:** The cart displays item prices, quantities, subtotals, and applicable fees.
- **FR-24:** Item availability and current prices are re-validated before an order is accepted (FR-24 ↔ BR-05).

### 9.5 Checkout and Order Placement

- **FR-25:** Customers can review their complete order (preview) before submission, with the final total (tax, delivery fees).
- **FR-26:** Customers provide the information required by the selected fulfilment method (delivery address for delivery).
- **FR-27:** The system calculates the order total from item prices, fees, and taxes.
- **FR-28:** The system creates the order (snapshotting items, prices, charges, and fees configuration) after validation succeeds.
- **FR-29:** Purchased item details, quantities, prices, and charges are preserved as agreed at checkout (BR-04).
- **FR-30:** Customers are informed whether their order was successfully submitted.
- **FR-31:** Unavailable items or invalid order totals are rejected.

### 9.6 Order Processing

- **FR-32:** New orders are made available to the owning restaurant.
- **FR-33:** Restaurant owners can accept or reject pending orders.
- **FR-34:** Restaurant owners update accepted orders as preparation and fulfilment progress (preparing → ready → completed; delivery status for delivery orders).
- **FR-35:** Only valid order-status transitions are enforced (BR-08).
- **FR-36:** Customers can view the current status of their orders.
- **FR-37:** Rejected and cancelled orders are recorded permanently.
- **FR-38:** Only the owning restaurant (or an administrator) can change an order's status (BR-10).

### 9.7 Order Cancellation

- **FR-39:** Customers can cancel pending orders with a reason.
- **FR-40:** Cancellation after restaurant acceptance follows the defined cancellation policy.
- **FR-41:** Cancellation is blocked once the policy no longer permits it.
- **FR-42:** Customers are informed of the cancellation outcome.

### 9.8 Order History

- **FR-43:** Customers can view their previous orders and details.
- **FR-44:** Restaurant owners can view historical orders belonging to their restaurant.
- **FR-45:** Administrators can review order records and payments for operational oversight.
- **FR-46:** Historical orders retain their original purchase details even when menu information changes (BR-04).

### 9.9 Administration

- **FR-47:** Administrators can review restaurant applications.
- **FR-48:** Administrators can approve or reject applications (rejection requires a reason).
- **FR-49:** Administrators can suspend or reinstate restaurants.
- **FR-50:** Administrators can manage customer and restaurant accounts, including locking/unlocking them.
- **FR-51:** Administrators can inspect order records and payments when handling operational issues.
- **FR-52:** Administrative operations are restricted to administrators.

## 10. Non-Functional Requirements

- **NFR-01 — Security:** Accounts and sensitive information are protected (Identity with hashed passwords, JWT access tokens, single-use refresh tokens, locked-account enforcement).
- **NFR-02 — Authorization:** Role + ownership permissions are enforced consistently across all protected operations.
- **NFR-03 — Usability:** Clear navigation and understandable feedback for customer, restaurant, and administrative actions.
- **NFR-04 — Responsiveness:** The frontend supports common desktop, tablet, and mobile screen sizes.
- **NFR-05 — Performance:** Common browsing and ordering operations respond promptly; reads are SQL-projected rather than loading full graphs.
- **NFR-06 — Reliability:** Failed operations do not leave orders in an inconsistent or partially confirmed state (unit of work per command).
- **NFR-07 — Data Integrity:** Order prices, quantities, totals, ownership, and statuses remain consistent (BR-02, BR-04, BR-11).
- **NFR-08 — Maintainability:** Clean Architecture keeps business rules separate from infrastructure and presentation (see `docs/backend-architecture.md`).
- **NFR-09 — Auditability:** Audited entities record creation/modification time and, where available, the modifying user.
- **NFR-10 — Privacy:** Personal information is accessible only to authorized users for legitimate purposes.

## 11. MVP Scope

The first version includes:

- Customer accounts (registration, email confirmation, sign-in, OTP-based recovery, profile).
- Restaurant discovery and menu browsing.
- Restaurant registration, administrator approval, suspension.
- Menu management (platform food categories, restaurant menu types, food items with images).
- Shopping cart and checkout (pickup/delivery, cash/online payment, price preview).
- Order placement, status tracking, and cancellation.
- Restaurant order management and owner storefront/dashboard.
- Payment records, the transaction ledger, and platform order/payment oversight.
- Basic administrative controls (applications, accounts, dashboard, food categories).

Advanced features such as loyalty programs, restaurant advertising, customer reviews, order notifications, and advanced analytics are candidates for future releases.

## 12. MVP Acceptance Criteria

The MVP is functionally acceptable when:

- Customers can register, confirm their email, sign in, browse restaurants, and view menus.
- Restaurant owners can apply for approval and manage their own menus.
- Administrators can approve or reject restaurants and manage participation and accounts.
- Customers can create a single-restaurant cart, preview the order, and submit valid orders.
- The system validates availability and calculates order totals consistently between preview and placement.
- Restaurants can accept, reject, and process orders through completion.
- Customers can view order status and history.
- Cancellation and transition rules are enforced consistently.
- Order records preserve the original purchase details.
- Unauthorized access to customer, restaurant, and administrative information is prevented.
- The core customer and restaurant workflows work across supported screen sizes.

## 13. Open Decisions

These decisions remain open and must be resolved before the related features are built:

1. **Revenue model** — whether Sofra charges commission or fees, and how (a placeholder fee/tax configuration feeds the platform today).
2. **Delivery responsibility** — restaurant-managed, platform-managed, or third-party couriers.
3. **Guest checkout** — account required, or ordering without an account.
4. **Cancellation policy** — exact rules after a restaurant accepts an order.
5. **Fees** — final amounts and formulas for delivery fees, service fees, and taxes (placeholder values in configuration).
6. **Notifications** — channels for order updates (in-app, email, SMS).
7. **Online payment gateway** — the online payment method is recorded but not yet integrated with a provider.
8. **Restaurant staff** — whether multiple staff accounts per restaurant are supported.

## 14. Product Direction

Sofra is developed as an **ASP.NET Core Web API** (no MVC views) following **Clean Architecture**, with a versioned REST interface (`/api/v1`), SQL Server persistence via EF Core, and a rich domain model. The React/TypeScript frontend is a separate application served in development by Vite.

- Architecture: [`docs/backend-architecture.md`](backend-architecture.md)
- Frontend: [`docs/frontend-architecture.md`](frontend-architecture.md)
- Database model: [`docs/erd.md`](erd.md)
- Endpoints: [`docs/API.md`](API.md)
