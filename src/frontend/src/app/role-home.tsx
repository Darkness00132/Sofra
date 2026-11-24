import { Navigate } from "react-router-dom"
import { useProfileQuery } from "@/features/auth/service/api"
import { PageLoader } from "@/shared/components/page-loader"
import { Roles } from "@/shared/constants/roles"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"

/**
 * Sends each signed-in role to its own home. Guests land in the marketplace.
 * Login responses omit roles, so this waits until the profile is known.
 */
export function RoleHome() {
  const authenticated = useAuthStore(selectIsAuthenticated)
  const user = useAuthStore((state) => state.user)
  const profile = useProfileQuery()

  if (!authenticated) {
    return <Navigate to="/restaurants" replace />
  }

  const roles = user?.roles.length ? user.roles : (profile.data?.roles ?? [])

  if (roles.length === 0) {
    if (profile.isError) {
      return <Navigate to="/restaurants" replace />
    }

    return (
      <div className="theme-market bg-background text-foreground">
        <PageLoader />
      </div>
    )
  }

  if (roles.includes(Roles.Administrator)) {
    return <Navigate to="/admin/dashboard" replace />
  }

  if (roles.includes(Roles.Restaurant)) {
    return <Navigate to="/restaurant/dashboard" replace />
  }

  return <Navigate to="/restaurants" replace />
}
