import type { RouteObject } from "react-router-dom"
import { MyRestaurantPage } from "./pages/my-restaurant-page"

/**
 * Route subtree for the owner's storefront. Authenticated — wrapped by the
 * app router.
 */
export const myRestaurantRoutes: RouteObject[] = [
  { path: "restaurants/me", element: <MyRestaurantPage /> },
]
