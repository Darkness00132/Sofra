import type { RouteObject } from "react-router-dom"
import { MenuPage } from "./pages/menu-page"

/**
 * Route subtree for the owner's menu management. Authenticated — wrapped by
 * the app router.
 */
export const menuRoutes: RouteObject[] = [
  { path: "menus", element: <MenuPage /> },
]
