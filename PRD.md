# Sofra — Product Requirements Document (PRD)

## 1. Product Overview

**Sofra** is a multi-vendor restaurant platform that allows customers to discover restaurants, browse menus, and order food from different restaurants through one website.

The platform provides restaurants with their own storefronts and tools to manage menus and incoming orders, while administrators oversee the marketplace.

## 2. Problem Statement

Customers need a convenient way to discover restaurants and order food online. Meanwhile, restaurants need an accessible platform to showcase their menus and manage online orders without building their own ordering websites.

Sofra brings both sides together in one marketplace.

## 3. Product Vision

To create a simple and accessible platform that connects customers with restaurants and makes online food ordering convenient for everyone.

## 4. Target Users

- **Customers:** People who want to discover restaurants and order food online.
- **Restaurant Owners:** Businesses that want to reach customers and manage online orders.
- **Platform Administrators:** People responsible for managing restaurants and overseeing the platform.

## 5. Core Features

### Customer Experience
- Browse and search for restaurants.
- Explore restaurant menus, food descriptions, and prices.
- Add food items to a shopping cart.
- Place and manage orders.
- Track order status.
- View previous orders.

### Restaurant Management
- Manage restaurant information and storefront details.
- Create and update menu categories and food items.
- Manage prices and item availability.
- Receive and process incoming orders.
- Update order statuses.
- View order history and basic sales information.

### Platform Administration
- Review and approve restaurant applications.
- Manage restaurant and customer accounts.
- Monitor orders and restaurant activity.
- Suspend restaurants when necessary.
- Oversee marketplace operations.

## 6. MVP Scope

The first version of Sofra will focus on the essential marketplace experience:

- Customer accounts.
- Restaurant discovery and menu browsing.
- Restaurant registration and administrator approval.
- Menu management.
- Shopping cart and checkout.
- Order placement and status tracking.
- Restaurant order management.
- Basic administrative controls.

Advanced features such as loyalty programs, restaurant advertising, customer reviews, and advanced analytics can be considered for future releases.

## 7. Product Goals

- Make discovering restaurants and ordering food straightforward.
- Enable restaurants to manage their online presence and orders.
- Provide a centralized marketplace for multiple restaurants.
- Establish a foundation for future product growth.

## 8. Success Criteria

Sofra's initial success will be evaluated through restaurant adoption, customer activity, completed orders, repeat customers, and overall ordering reliability.

## 9. Key Decisions to Finalize

Before development begins, the following business decisions must be confirmed:

- Will Sofra support delivery, pickup, or both?
- Will payments be online, cash on delivery, or both?
- Will each order belong to a single restaurant?
- How will Sofra generate revenue, if applicable?

## 10. Product Direction

Sofra will be developed as a web application using ASP.NET Core MVC, following Clean Architecture principles.

The initial priority is to deliver a functional marketplace that supports the complete journey from restaurant discovery to order fulfilment. Technical design and implementation details will be addressed separately.