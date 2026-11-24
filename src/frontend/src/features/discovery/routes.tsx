import type { RouteObject } from "react-router-dom"
import { RestaurantDetailsPage } from "./pages/restaurant-details-page"
import { RestaurantsPage } from "./pages/restaurants-page"

/** Public route subtree for restaurant discovery — no authentication required. */
export const discoveryRoutes: RouteObject[] = [
  { path: "restaurants", element: <RestaurantsPage /> },
  { path: "restaurants/:restaurantId", element: <RestaurantDetailsPage /> },
]
