import { create } from "zustand"
import { persist } from "zustand/middleware"

/**
 * The user snapshot the session carries. Defined structurally here (shared
 * must not import feature schemas); feature z.infer types satisfy it.
 */
export interface AuthUser {
  id: string
  email: string
  roles: string[]
  phoneNumber: string | null
}

interface AuthTokens {
  accessToken: string
  refreshToken: string
  /** ISO-8601 UTC string — persist-friendly; map to Date at the edges. */
  accessTokenExpiresAtUtc: string
}

interface AuthState {
  accessToken: string | null
  refreshToken: string | null
  /** ISO-8601 UTC string — persist-friendly; map to Date at the edges. */
  accessTokenExpiresAtUtc: string | null
  user: AuthUser | null
  /** Sign-in / registration: fresh tokens + the user they identify. */
  setSession: (session: AuthTokens & { user: AuthUser }) => void
  /** Token rotation only (the shared client's refresh flow) — keeps `user`. */
  setTokens: (tokens: AuthTokens) => void
  setUser: (user: AuthUser) => void
  clearSession: () => void
}

/**
 * The single source of truth for the auth session. zustand stores are plain
 * objects, so the axios client reads and mutates them outside React via
 * getState() (shared/api/client.ts) while components subscribe through
 * selectors.
 */
export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      accessToken: null,
      refreshToken: null,
      accessTokenExpiresAtUtc: null,
      user: null,
      setSession: ({
        accessToken,
        refreshToken,
        accessTokenExpiresAtUtc,
        user,
      }) => set({ accessToken, refreshToken, accessTokenExpiresAtUtc, user }),
      setTokens: ({ accessToken, refreshToken, accessTokenExpiresAtUtc }) =>
        set({ accessToken, refreshToken, accessTokenExpiresAtUtc }),
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
