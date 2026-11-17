# Sofra — Product Requirements Document (PRD)

**Version:** 2.0 — updated 2026-11-17 to reflect resolved product decisions and the implemented domain
**Related documents:** `docs/SRS.md` (functional requirements & business rules BR-01…BR-11)

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

## 5. Core Features

### Customer Experience

- Discover restaurants: browse, search by name, and filter.
- Explore menus: platform-level food categories (e.g. Burger, Pizza), each restaurant's own menu types under those categories, and food items with descriptions, prices, and availability.
- Shop with a cart that holds items from **one restaurant at a time**; adding from another restaurant requires clearing the cart first.
- Check out with a chosen fulfilment method — **pickup or delivery** — and pay by **cash or online payment**.
- Place orders and watch them progress through the order lifecycle, including delivery tracking.
- Cancel orders while the applicable cancellation policy still allows it.
- Review previous orders with their original prices and details.

### Restaurant Management

- Join via an **application** reviewed by administrators; the restaurant storefront is created only after approval.
- Manage restaurant information (name, address, phone, description).
- Build the menu: create menu types under platform food categories and manage food items (name, description, price, availability).
- Toggle whether the restaurant is accepting orders.
- Receive incoming orders and decide: accept or reject.
- Process accepted orders through preparation until completion.
- View order history with original purchase details.

### Platform Administration

- Review restaurant applications; approve or reject (with a reason).
- Suspend or reinstate restaurants; a suspended restaurant cannot accept orders.
- Manage customer and restaurant accounts.
- Inspect order records for operational oversight.

## 6. Ordering Model (decided)

The following product decisions are **resolved** and reflected in the platform design:

| Decision | Resolution |
|---|---|
| Fulfilment methods | **Pickup and Delivery** — customer chooses at checkout; delivery requires an address |
| Payment methods | **Cash and Online payment** — chosen at checkout |
| Order ownership | **One restaurant per order** (and one restaurant per cart) |
| Price integrity | Item prices are **locked at order time**; later menu changes never alter past orders |
| Order lifecycle | **Pending → Accepted → Preparing → Ready → Completed**, with **Rejected** and **Cancelled** as terminal outcomes |
| Delivery tracking | Delivery orders track their own delivery status (collected → delivered), separate from the order status |
| Payment recording | Every order payment moves through its own payment record (pending → completed/failed/refunded/cancelled) |
| Money movements | A transaction ledger records debits and credits tied to orders, payments, and restaurants |

## 7. MVP Scope

The first version of Sofra focuses on the essential marketplace experience:

- Customer accounts (registration, sign-in, profile).
- Restaurant discovery and menu browsing.
- Restaurant registration and administrator approval.
- Menu management (categories, restaurant menu types, food items).
- Shopping cart and checkout (pickup/delivery, cash/online payment).
- Order placement, status tracking, and cancellation.
- Restaurant order management.
- Payment records and the transaction ledger.
- Basic administrative controls.

Advanced features such as loyalty programs, restaurant advertising, customer reviews, and advanced analytics are candidates for future releases.

## 8. Product Goals

- Make discovering restaurants and ordering food straightforward.
- Enable restaurants to manage their online presence and orders.
- Provide a centralized marketplace for multiple restaurants.
- Establish a foundation for future product growth.

## 9. Success Criteria

Sofra's initial success will be evaluated through restaurant adoption, customer activity, completed orders, repeat customers, and overall ordering reliability.

## 10. Open Decisions

These decisions remain open and must be resolved before the related features are implemented:

1. **Revenue model** — whether Sofra charges commission or fees, and how.
2. **Delivery responsibility** — restaurant-managed, platform-managed, or third-party couriers.
3. **Guest checkout** — account required, or ordering without an account.
4. **Cancellation policy** — exact rules after a restaurant accepts an order.
5. **Fees** — amounts and formulas for delivery fees, service fees, and taxes.
6. **Notifications** — channels for order updates (in-app, email, SMS).
7. **Restaurant staff** — whether multiple staff accounts per restaurant are supported.

## 11. Product Direction

Sofra is developed as an **ASP.NET Core Web API** application following **Clean Architecture** principles, with a versioned REST interface, SQL Server persistence, and a rich domain model. Detailed technical requirements and behavior live in `docs/SRS.md`; implementation conventions live with the code.
