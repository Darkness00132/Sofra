import type { RouteObject } from "react-router-dom"
import { OwnerDashboardPage } from "./pages/owner-dashboard-page"
import { RestaurantOrderDetailsPage } from "./pages/restaurant-order-details-page"
import { RestaurantOrdersPage } from "./pages/restaurant-orders-page"

/**
 * Route subtree for the owner's order processing and sales dashboard.
 * Authenticated — wrapped by the app router.
 */
export const restaurantOrderRoutes: RouteObject[] = [
  { path: "restaurant/orders", element: <RestaurantOrdersPage /> },
  { path: "restaurant/orders/:orderId", element: <RestaurantOrderDetailsPage /> },
  { path: "restaurant/dashboard", element: <OwnerDashboardPage /> },
]
