import type { RouteObject } from "react-router-dom"
import { CheckoutPage } from "./pages/checkout-page"
import { OrderDetailsPage } from "./pages/order-details-page"
import { OrdersPage } from "./pages/orders-page"

/**
 * Route subtree for the customer's orders: checkout, history and details.
 * Authenticated — wrapped by the app router.
 */
export const orderRoutes: RouteObject[] = [
  { path: "checkout", element: <CheckoutPage /> },
  { path: "orders", element: <OrdersPage /> },
  { path: "orders/:orderId", element: <OrderDetailsPage /> },
]
