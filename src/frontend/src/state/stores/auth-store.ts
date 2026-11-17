import { create } from "zustand"
import { persist } from "zustand/middleware"
import type { AuthSession, AuthUser } from "@/models/domain/auth.models"

interface AuthState {
  accessToken: string | null
  refreshToken: string | null
  /** ISO-8601 UTC string — persist-friendly; map to Date at the edges. */
  accessTokenExpiresAtUtc: string | null
  user: AuthUser | null
  setSession: (session: AuthSession) => void
  setUser: (user: AuthUser) => void
  clearSession: () => void
}

/**
 * The single source of truth for the auth session. zustand stores are plain
 * objects, so the axios client reads and mutates them outside React via
 * getState() (api/client.ts) while components subscribe through selectors.
 */
export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      accessTokenExpiresAtUtc: null,
      user: null,
      setSession: (session) =>
        set({
          accessToken: session.accessToken,
          refreshToken: session.refreshToken,
          accessTokenExpiresAtUtc: session.accessTokenExpiresAt.toISOString(),
          user: session.user,
        }),
      setUser: (user) => set({ user }),
      clearSession: () =>
        set({
          accessToken: null,
          refreshToken: null,
          accessTokenExpiresAtUtc: null,
          user: null,
        }),
    }),
    { name: "sofra.auth" }
  )
)

export const selectIsAuthenticated = (state: AuthState): boolean =>
  state.accessToken !== null
