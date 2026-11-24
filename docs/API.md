# Sofra API — Endpoints & Business Flow

Base path: `api/v1/...`. Auth: JWT bearer. Roles: **Customer**, **Restaurant owner**, **Administrator**.

## Statuses (quick reference)

- **Order:** Pending → Accepted → Preparing → Ready → Completed (or Cancelled / Rejected at early stages).
- **Restaurant:** PendingApproval → Approved (can be Suspended / Reinstated).
- **Application:** Pending → Approved (creates the restaurant) or Rejected.
- **Payment:** Pending → Completed / Failed / Cancelled / Refunded.
- **Food item:** Available / Unavailable.

## Authentication

| Method | Path | When to use |
|---|---|---|
| POST | `/auth/register` | New customer sign-up |
| POST | `/auth/login` | Sign in |
| POST | `/auth/refresh` | Access token expired — get a new pair |
| POST | `/auth/logout` | End the session (revokes refresh token) |
| POST | `/auth/forgot-password` | Request a password-reset OTP by email |
| POST | `/auth/reset-password` | Reset password using the OTP |
| POST | `/auth/verify-email` | Verify email using the token from registration |
| GET | `/auth/me` | Read the signed-in user's profile |
| PUT | `/auth/me` | Update profile |

**Flow:** register/login → store both tokens → send access token on every call → refresh when it expires → logout on sign-out.

## Cart (customer)

| Method | Path | When to use |
|---|---|---|
| GET | `/cart` | View current cart (items, restaurant, subtotal) |
| POST | `/cart/items` | Add an item — cart holds one restaurant at a time; adding from another restaurant returns 409 (clear first) |
| PUT | `/cart/items/{foodItemId}` | Change quantity of an item already in the cart |
| DELETE | `/cart/items/{foodItemId}` | Remove one item |
| DELETE | `/cart` | Empty the cart (e.g. to switch restaurants) |

**Flow:** browse a restaurant's menu → add items → adjust quantities → optionally clear to switch restaurants → checkout creates the order.

## Orders — Customer

| Method | Path | When to use |
|---|---|---|
| POST | `/orders/preview` | Check totals/fees before placing |
| POST | `/orders` | Place the order from the current cart |
| GET | `/orders` | List my orders |
| GET | `/orders/{orderId}` | View order details |
| POST | `/orders/{orderId}/cancel` | Cancel — only while still Pending |

**Flow:** preview → place order (cart becomes the order) → track status → cancel only if Pending.

## Orders — Restaurant owner

| Method | Path | When to use |
|---|---|---|
| GET | `/restaurants/orders` | Incoming orders for my restaurant |
| GET | `/restaurants/orders/{orderId}` | Order details |
| POST | `/restaurants/orders/{orderId}/status` | Accept / reject / mark preparing / ready / completed |
| GET | `/restaurants/dashboard` | My sales summary (counts + revenue) |

**Flow:** see new order → accept → prepare → mark ready → complete. Reject only early. Dashboard summarizes performance.

## Restaurants — Discovery (public)

| Method | Path | When to use |
|---|---|---|
| GET | `/restaurants` | Browse approved restaurants (search/filter) |
| GET | `/restaurants/{restaurantId}` | Restaurant details |
| GET | `/restaurants/{restaurantId}/menu` | Public menu (available items only) |

## Restaurant — Owner management (`/restaurants/me`)

| Method | Path | When to use |
|---|---|---|
| GET | `/restaurants/me` | View my restaurant |
| PUT | `/restaurants/me` | Update info |
| PATCH | `/restaurants/me` | Partial update |
| POST | `/restaurants/me/image` | Upload restaurant image |
| DELETE | `/restaurants/me/image` | Remove image |

Only available after the application is approved.

## Menu — Owner (`/restaurants/me/menu`)

| Method | Path | When to use |
|---|---|---|
| GET | `/restaurants/me/menu` | Full menu including unavailable items |
| POST | `/restaurants/me/menu/types` | Add a menu category (e.g. "Burgers") |
| PUT | `/restaurants/me/menu/types/{menuTypeId}` | Rename/reorder a category |
| DELETE | `/restaurants/me/menu/types/{menuTypeId}` | Remove category (only if empty) |
| POST | `/restaurants/me/menu/items` | Add a food item |
| PUT | `/restaurants/me/menu/items/{foodItemId}` | Edit item details/price |
| DELETE | `/restaurants/me/menu/items/{foodItemId}` | Delete item — blocked if it appears in past orders; mark unavailable instead |
| PATCH | `/restaurants/me/menu/items/{foodItemId}/availability` | Toggle available/unavailable |
| POST | `/restaurants/me/menu/items/{foodItemId}/image` | Upload item image |
| DELETE | `/restaurants/me/menu/items/{foodItemId}/image` | Remove item image |

## Restaurant applications

| Method | Path | When to use |
|---|---|---|
| POST | `/restaurants/apply` | Submit an application to become a restaurant (one pending at a time) |
| GET | `/restaurants/my-applications` | Track my application status |

**Flow:** customer applies → admin reviews → on approval a Restaurant is created and the user gets the Restaurant role.

## Admin — Application review (`/admin/restaurant-applications`)

| Method | Path | When to use |
|---|---|---|
| GET | `/admin/restaurant-applications` | List applications (filter by status) |
| GET | `/admin/restaurant-applications/{applicationId}` | Application details |
| POST | `/admin/restaurant-applications/{applicationId}/status` | Approve or reject (with reason) |

## Admin — Restaurants (`/admin/restaurants`)

| Method | Path | When to use |
|---|---|---|
| GET | `/admin/restaurants` | List restaurants (filter) |
| POST | `/admin/restaurants/{restaurantId}/status` | Suspend or reinstate a restaurant |

## Admin — Platform data

| Method | Path | When to use |
|---|---|---|
| GET | `/admin/dashboard` | Platform counts + sales summary |
| GET | `/admin/accounts` | List accounts |
| PATCH | `/admin/accounts/{accountId}/lock` | Lock/unlock an account |
| GET | `/admin/orders` | List all orders (filter) |
| GET | `/admin/orders/{orderId}` | Order details |
| GET | `/admin/payments` | List payments (filter) |
| GET/POST/PUT/DELETE | `/admin/food-categories` | Manage platform food categories (e.g. Burger, Pizza) |

## Public food categories

| Method | Path | When to use |
|---|---|---|
| GET | `/food-categories` | List platform food categories |

---

**End-to-end flow:** customer registers → browses restaurants/menus → builds a single-restaurant cart → previews & places order → tracks/cancels while pending. Restaurant owner applies → admin approves → owner manages menu & orders. Admin polices restaurants, accounts, orders, payments, and categories.
