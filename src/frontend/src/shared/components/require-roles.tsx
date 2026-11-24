import type { ReactNode } from "react"
import { Link } from "react-router-dom"
import { useAuthStore } from "@/shared/stores/auth.store"

interface RequireRolesProps {
  /** The roles that may see the children (any-of). */
  roles: readonly string[]
  children: ReactNode
}

/**
 * Role guard for role-scoped features (admin/owner pages). Relies on the
 * profile being synced into the auth store by the app shell's profile sync —
 * while roles are unknown it renders a loader instead of wrongly denying.
 */
export function RequireRoles({ roles, children }: RequireRolesProps) {
  const user = useAuthStore((state) => state.user)

  // Roles not yet synced (fresh login, profile query in flight) — wait.
  if (user === null || user.roles.length === 0) {
    return null
  }

  const allowed = user.roles.some((role) => roles.includes(role))

  if (!allowed) {
    return (
      <main className="mx-auto flex min-h-svh max-w-md items-center justify-center p-4">
        <p className="text-muted-foreground">
          You do not have access to this page.
        </p>
        <Link to="/" className="ml-2 text-sm hover:underline">
          Back home
        </Link>
      </main>
    )
  }

  return children
}
