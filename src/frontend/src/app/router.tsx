import { Outlet, createBrowserRouter, Navigate } from "react-router-dom"
import { AdminLayout } from "./admin-layout"
import { AppLayout } from "./app-layout"
import { AuthLayout } from "./auth-layout"
import { OwnerLayout } from "./owner-layout"
import { RoleHome } from "./role-home"
import { guestAuthRoutes, profileRoute } from "@/features/auth/routes"
import { cartRoutes } from "@/features/carts/routes"
import { discoveryRoutes } from "@/features/discovery/routes"
import { menuRoutes } from "@/features/menus/routes"
import { orderRoutes } from "@/features/orders/routes"
import { myRestaurantRoutes } from "@/features/my-restaurant/routes"
import { restaurantApplicationRoutes } from "@/features/restaurant-applications/routes"
import { restaurantOrderRoutes } from "@/features/restaurant-orders/routes"
import { adminRoutes } from "@/features/admin/routes"
import { RequireAuth } from "@/shared/components/require-auth"
import { RequireRoles } from "@/shared/components/require-roles"
import { Roles } from "@/shared/constants/roles"

export const router = createBrowserRouter([
  // Guest auth routes (login, register, forgot password)
  {
    element: <AuthLayout />, // public auth pages
    children: guestAuthRoutes,
  },
  // Root redirects based on role
  { path: "/", element: <RoleHome /> },
  // Customer marketplace layout
  {
    element: <AppLayout />, // provides header, footer, navigation for customers
    children: [
      ...discoveryRoutes,
      {
        element: <RequireAuth><Outlet /></RequireAuth>,
        children: [profileRoute, ...cartRoutes, ...orderRoutes, ...restaurantApplicationRoutes],
      },
    ],
  },
  // Restaurant owner layout
  {
    element: (
      <RequireAuth>
        <RequireRoles roles={[Roles.Restaurant]}>
          <OwnerLayout />
        </RequireRoles>
      </RequireAuth>
    ),
    children: [...myRestaurantRoutes, ...menuRoutes, ...restaurantOrderRoutes],
  },
  // Admin layout
  {
    element: (
      <RequireAuth>
        <RequireRoles roles={[Roles.Administrator]}>
          <AdminLayout />
        </RequireRoles>
      </RequireAuth>
    ),
    children: adminRoutes,
  },
  // Catch‑all redirect
  { path: "*", element: <Navigate to="/" replace /> },
]);