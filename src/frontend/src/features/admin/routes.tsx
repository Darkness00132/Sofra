import type { RouteObject } from "react-router-dom"
import { AdminAccountsPage } from "./pages/admin-accounts-page"
import { AdminDashboardPage } from "./pages/admin-dashboard-page"
import { AdminOrderDetailsPage } from "./pages/admin-order-details-page"
import { AdminOrdersPage } from "./pages/admin-orders-page"
import { AdminPaymentsPage } from "./pages/admin-payments-page"
import { AdminRestaurantsPage } from "./pages/admin-restaurants-page"
import { ApplicationsReviewPage } from "./pages/applications-review-page"

/**
 * Route subtree for administrator features. The app router wraps these with
 * RequireAuth + RequireRoles(Administrator).
 */
export const adminRoutes: RouteObject[] = [
  { path: "admin/dashboard", element: <AdminDashboardPage /> },
  { path: "admin/restaurant-applications", element: <ApplicationsReviewPage /> },
  { path: "admin/restaurants", element: <AdminRestaurantsPage /> },
  { path: "admin/orders", element: <AdminOrdersPage /> },
  { path: "admin/orders/:orderId", element: <AdminOrderDetailsPage /> },
  { path: "admin/accounts", element: <AdminAccountsPage /> },
  { path: "admin/payments", element: <AdminPaymentsPage /> },
]
