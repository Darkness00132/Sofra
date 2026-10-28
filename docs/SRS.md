# Sofra
## Software Requirements Specification (SRS)

**Version:** 1.0  
**Product:** Multi-Vendor Restaurant Ordering Platform  
**Application Type:** Web Application  
**Technology:** ASP.NET Core MVC with Clean Architecture

## 1. Introduction

### 1.1 Purpose

This document defines the functional and non-functional requirements for Sofra's initial release (MVP). It establishes what the system must do, the rules governing its operation, and the expected behavior of its users.

### 1.2 Product Scope

Sofra allows customers to discover restaurants, browse menus, and place food orders through a single platform. Restaurants manage their storefronts and orders, while administrators oversee restaurant participation and marketplace operations.

The MVP focuses on the core ordering process, from restaurant discovery to order completion.

### 1.3 Intended Users

- Customers
- Restaurant owners and authorized staff
- Platform administrators

## 2. Overall Description

### 2.1 Product Functions

The system shall provide:

- User registration, authentication, and account management.
- Restaurant discovery and menu browsing.
- Restaurant applications and approval.
- Menu and availability management.
- Shopping cart and checkout.
- Order processing and status tracking.
- Order history.
- Administrative oversight.

### 2.2 User Roles

**Customer:** Browses restaurants, manages a cart, places orders, and views order history.

**Restaurant Owner/Staff:** Manages restaurant information, menu items, availability, and incoming orders.

**Administrator:** Approves restaurants, manages accounts, and monitors platform operations.

Each role shall have access only to the operations permitted for that role.

## 3. Functional Requirements

### 3.1 Account Management

**FR-01:** The system shall allow customers to register using the supported registration method.

**FR-02:** The system shall allow registered users to sign in and sign out.

**FR-03:** The system shall allow users to update permitted profile information.

**FR-04:** The system shall restrict protected operations to authenticated and authorized users.

**FR-05:** The system shall allow users to recover access to their accounts through a supported recovery process.

### 3.2 Restaurant Discovery

**FR-06:** The system shall display approved restaurants that are eligible to appear on the platform.

**FR-07:** Customers shall be able to view restaurant details, menus, prices, and availability.

**FR-08:** Customers shall be able to search for restaurants by name.

**FR-09:** Customers shall be able to filter restaurants using supported criteria, such as cuisine type.

**FR-10:** The system shall clearly indicate when a restaurant is not accepting orders.

### 3.3 Restaurant Management

**FR-11:** Restaurant owners shall be able to submit applications to join Sofra.

**FR-12:** Authorized restaurant users shall be able to update their restaurant's permitted information.

**FR-13:** Restaurant users shall be able to create, update, and remove menu categories.

**FR-14:** Restaurant users shall be able to create, update, and remove food items.

**FR-15:** Food items shall include a name, description, price, and availability status.

**FR-16:** Restaurant users shall be able to mark food items as available or unavailable.

**FR-17:** Restaurant users shall be able to control whether their restaurant is accepting orders.

**FR-18:** Restaurant users shall only manage their own restaurant's information and orders.

### 3.4 Shopping Cart

**FR-19:** Customers shall be able to add food items to their shopping cart.

**FR-20:** Customers shall be able to change item quantities and remove items before checkout.

**FR-21:** The cart shall contain items from only one restaurant at a time.

**FR-22:** If a customer attempts to add an item from another restaurant, the system shall require the customer to confirm before replacing the existing cart.

**FR-23:** The system shall display item prices, quantities, subtotals, and applicable fees in the cart.

**FR-24:** The system shall validate item availability and current prices before accepting an order.

### 3.5 Checkout and Order Placement

**FR-25:** The system shall allow customers to review their complete order before submission.

**FR-26:** Customers shall provide the information required by the selected fulfilment method.

**FR-27:** The system shall calculate the order total using applicable prices, fees, taxes, and discounts.

**FR-28:** The system shall create an order after the required validation and submission steps succeed.

**FR-29:** The system shall preserve the purchased item details, quantities, prices, and applicable charges as they were agreed upon at checkout.

**FR-30:** The system shall inform customers whether their order was successfully submitted.

**FR-31:** The system shall prevent unavailable items or invalid order totals from being accepted.

### 3.6 Order Processing

**FR-32:** The system shall make new orders available to the relevant restaurant.

**FR-33:** Restaurant users shall be able to accept or reject pending orders.

**FR-34:** Restaurant users shall be able to update accepted orders as preparation progresses.

**FR-35:** The system shall enforce valid order-status transitions.

**FR-36:** Customers shall be able to view the current status of their orders.

**FR-37:** The system shall record rejected and cancelled orders for future reference.

**FR-38:** The system shall prevent unauthorized users from modifying order statuses.

### 3.7 Order Cancellation

**FR-39:** Customers shall be able to cancel pending orders.

**FR-40:** Cancellation requests after restaurant acceptance shall follow the defined cancellation policy.

**FR-41:** The system shall prevent cancellation when the applicable policy no longer permits it.

**FR-42:** The system shall communicate the outcome of a cancellation request to the customer.

### 3.8 Order History

**FR-43:** Customers shall be able to view their previous orders and relevant order details.

**FR-44:** Restaurant users shall be able to view historical orders belonging to their restaurant.

**FR-45:** Administrators shall be able to review order records for operational oversight.

**FR-46:** Historical orders shall retain their original purchase details even when menu information changes.

### 3.9 Administration

**FR-47:** Administrators shall be able to review restaurant applications.

**FR-48:** Administrators shall be able to approve or reject restaurant applications.

**FR-49:** Administrators shall be able to suspend or reactivate restaurant participation.

**FR-50:** Administrators shall be able to manage customer and restaurant accounts within their permissions.

**FR-51:** Administrators shall be able to inspect order records when handling operational issues.

**FR-52:** The system shall restrict administrative operations to authorized administrators.

## 4. Business Rules

**BR-01 — Restaurant Approval:** Restaurants must be approved before accepting customer orders.

**BR-02 — Single-Restaurant Orders:** Every order shall belong to exactly one restaurant.

**BR-03 — Restaurant Data Isolation:** Restaurant users may only manage their own restaurant's operational information.

**BR-04 — Price Integrity:** Menu price changes shall not alter the prices recorded in existing orders.

**BR-05 — Availability:** Unavailable food items cannot be ordered.

**BR-06 — Order Acceptance:** An order remains pending until the restaurant accepts or rejects it.

**BR-07 — Order Cancellation:** Cancellation is governed by the order's status and the defined cancellation policy.

**BR-08 — Order Completion:** Only permitted status transitions may occur, and an order cannot be completed before the required fulfilment steps.

**BR-09 — Restaurant Suspension:** A suspended restaurant cannot accept new orders.

**BR-10 — Order Ownership:** Customers may access only their own orders, except for authorized administrative access.

**BR-11 — Total Calculation:** Order totals must accurately reflect the applicable prices and charges confirmed at checkout.

## 5. Order Lifecycle

The standard order lifecycle shall be:

1. **Pending:** The order has been submitted and awaits the restaurant's decision.
2. **Accepted:** The restaurant has agreed to fulfil the order.
3. **Preparing:** The restaurant is preparing the food.
4. **Ready:** The order is ready for fulfilment.
5. **Completed:** The order has been fulfilled.

Alternative terminal outcomes include:

- **Rejected:** The restaurant declines the order.
- **Cancelled:** The order is cancelled under the applicable policy.

If delivery is included in the MVP, an additional delivery status may be introduced. The exact lifecycle must reflect the selected fulfilment model.

## 6. Non-Functional Requirements

**NFR-01 — Security:** The system shall protect accounts and sensitive information against unauthorized access.

**NFR-02 — Authorization:** Permissions shall be enforced consistently across all protected operations.

**NFR-03 — Usability:** The website shall provide clear navigation and understandable feedback for customer, restaurant, and administrative actions.

**NFR-04 — Responsiveness:** The website shall support common desktop, tablet, and mobile screen sizes.

**NFR-05 — Performance:** Common browsing and ordering operations shall meet performance targets established before release.

**NFR-06 — Reliability:** Failed operations shall not leave orders in an inconsistent or partially confirmed state.

**NFR-07 — Data Integrity:** Order prices, quantities, totals, ownership, and statuses shall remain consistent.

**NFR-08 — Maintainability:** The system shall follow Clean Architecture principles to keep business rules separated from infrastructure and presentation concerns.

**NFR-09 — Auditability:** Important administrative and order-status changes shall be traceable when required for operational support.

**NFR-10 — Privacy:** Personal information shall only be accessible to authorized users for legitimate purposes.

## 7. MVP Acceptance Criteria

The MVP shall be considered functionally acceptable when:

- Customers can register, sign in, browse restaurants, and view menus.
- Restaurant owners can apply for approval and manage their own menus.
- Administrators can approve restaurants and manage their participation.
- Customers can create a single-restaurant cart and submit valid orders.
- The system validates availability and calculates order totals correctly.
- Restaurants can accept, reject, and process orders.
- Customers can view order status and history.
- Cancellation rules are enforced consistently.
- Order records preserve the original purchase details.
- Unauthorized access to customer, restaurant, and administrative information is prevented.
- The core customer and restaurant workflows work reliably across supported screen sizes.

## 8. Assumptions and Pending Decisions

The following decisions must be finalized before implementation:

1. **Fulfilment:** Delivery, pickup, or both.
2. **Payments:** Cash on delivery, online payments, or both.
3. **Delivery responsibility:** Restaurant-managed, platform-managed, or third-party.
4. **Customer checkout:** Account required or guest checkout supported.
5. **Cancellation:** Exact rules after restaurant acceptance.
6. **Fees:** Applicable delivery fees, service fees, taxes, and discounts.
7. **Notifications:** Required channels for order updates.
8. **Restaurant staff:** Whether multiple staff accounts are supported in the MVP.

These decisions affect the final requirements and must be resolved before the relevant features are implemented.

## 9. Technical Constraints

- The application shall use ASP.NET Core MVC.
- The application shall follow Clean Architecture.
- The system shall be delivered as a web application in the initial release.
- Detailed database design, application structure, interfaces, and deployment decisions shall be documented separately.

## 10. Conclusion

This SRS defines the expected behavior and business rules of Sofra's MVP. It provides a common reference for product planning, development, testing, and acceptance.

The implementation should begin only after the outstanding business decisions are resolved and the MVP scope is agreed upon.