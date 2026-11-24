import type { RouteObject } from "react-router-dom"
import { ApplyPage } from "./pages/apply-page"
import { MyApplicationsPage } from "./pages/my-applications-page"

/**
 * Route subtree for restaurant applications. These are authenticated routes —
 * the app router wraps them with RequireAuth.
 */
export const restaurantApplicationRoutes: RouteObject[] = [
  { path: "restaurants/apply", element: <ApplyPage /> },
  { path: "restaurants/my-applications", element: <MyApplicationsPage /> },
]
