import { useProfileQuery } from "@/features/auth/service/api"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"

/**
 * Keeps the auth store's user snapshot (roles, phone) fresh while signed in.
 * Renders nothing — the layout provides the visible chrome.
 */
export function ProfileSync() {
  const authenticated = useAuthStore(selectIsAuthenticated)
  useProfileQuery()

  void authenticated

  return null
}
