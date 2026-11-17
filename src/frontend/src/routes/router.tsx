import { createBrowserRouter, Navigate } from "react-router-dom"
import { RequireAuth } from "@/components/shared/require-auth"
import { HomePage } from "@/pages/home-page"
import { authRoutes } from "@/routes/auth-routes"

export const router = createBrowserRouter([
  {
    path: "/",
    element: (
      <RequireAuth>
        <HomePage />
      </RequireAuth>
    ),
  },
  ...authRoutes,
  { path: "*", element: <Navigate to="/" replace /> },
])
