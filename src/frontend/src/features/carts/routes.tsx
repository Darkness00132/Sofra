import type { RouteObject } from "react-router-dom"
import { CartPage } from "./pages/cart-page"

/** Route subtree for the customer's cart. Authenticated — wrapped by the app router. */
export const cartRoutes: RouteObject[] = [
  { path: "cart", element: <CartPage /> },
]
