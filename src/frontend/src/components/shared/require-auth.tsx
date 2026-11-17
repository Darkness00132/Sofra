import type { ReactNode } from "react"
import { Navigate, useLocation } from "react-router-dom"
import { selectIsAuthenticated, useAuthStore } from "@/state/stores/auth-store"

/**
 * Guard: renders children only for signed-in users; otherwise redirects to
 * /login, remembering the requested path for the post-login bounce back.
 */
export function RequireAuth({ children }: { children: ReactNode }) {
  const authenticated = useAuthStore(selectIsAuthenticated)
  const location = useLocation()

  if (!authenticated) {
    return (
      <Navigate
        to={`/login?redirectTo=${encodeURIComponent(location.pathname)}`}
        replace
      />
    )
  }

  return children
}
