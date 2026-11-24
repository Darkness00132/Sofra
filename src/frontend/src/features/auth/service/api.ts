import { useEffect } from "react"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"
import { api } from "@/shared/api/client"
import { selectIsAuthenticated, useAuthStore } from "@/shared/stores/auth.store"
import {
  loginResponseSchema,
  userProfileResponseSchema,
  type ForgotPasswordRequest,
  type LoginRequest,
  type LoginResponse,
  type LogoutRequest,
  type RegisterRequest,
  type ResetPasswordRequest,
  type UserProfile,
} from "../models/schemas"

/**
 * The auth feature's data module: pure endpoint functions (every response
 * zod-parsed at the boundary), its query-key registry, and the colocated
 * TanStack Query hooks. Session side effects go through the shared auth
 * store; the access-token refresh round-trip lives in shared/api/client.ts.
 */
// ─── Query keys ──────────────────────────────────────────────────────────────

export const authQueryKeys = {
  profile: () => ["auth", "profile"] as const,
}

// ─── Endpoint functions ──────────────────────────────────────────────────────

export async function registerAccount(
  request: RegisterRequest
): Promise<LoginResponse> {
  const response = await api.post("/auth/register", request)
  return loginResponseSchema.parse(response.data)
}

export async function signIn(request: LoginRequest): Promise<LoginResponse> {
  const response = await api.post("/auth/login", request)
  return loginResponseSchema.parse(response.data)
}

export async function signOut(request: LogoutRequest): Promise<void> {
  await api.post("/auth/logout", request)
}

export async function sendPasswordResetCode(
  request: ForgotPasswordRequest
): Promise<void> {
  await api.post("/auth/forgot-password", request)
}

export async function resetPassword(
  request: ResetPasswordRequest
): Promise<void> {
  await api.post("/auth/reset-password", request)
}

export async function fetchUserProfile(): Promise<UserProfile> {
  const response = await api.get("/auth/me")
  return userProfileResponseSchema.parse(response.data)
}

export async function updateProfile(
  request: { phoneNumber: string }
): Promise<UserProfile> {
  const response = await api.put("/auth/me", request)
  return userProfileResponseSchema.parse(response.data)
}

export async function updateProfileRequest(
  request: { phoneNumber: string }
): Promise<UserProfile> {
  return updateProfile(request)
}


// ─── Mutations ───────────────────────────────────────────────────────────────

/**
 * Login and register identify the account without roles. The store needs
 * those roles before navigation, or every user lands in the marketplace.
 */
async function openSession(
  response: LoginResponse,
  setSession: (session: {
    accessToken: string
    refreshToken: string
    accessTokenExpiresAtUtc: string
    user: {
      id: string
      email: string
      roles: string[]
      phoneNumber: string | null
    }
  }) => void,
  setUser: (user: UserProfile) => void,
  queryClient: ReturnType<typeof useQueryClient>
) {
  setSession({
    accessToken: response.accessToken,
    refreshToken: response.refreshToken,
    accessTokenExpiresAtUtc: response.accessTokenExpiresAtUtc,
    user: { ...response.user, roles: [], phoneNumber: null },
  })

  try {
    const profile = await fetchUserProfile()
    setUser(profile)
    queryClient.setQueryData(authQueryKeys.profile(), profile)
  } catch {
    // ProfileSync retries once the shell mounts.
  }
}

/** Sign in and store the returned session, including roles. */
export function useLoginMutation() {
  const setSession = useAuthStore((state) => state.setSession)
  const setUser = useAuthStore((state) => state.setUser)
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (request: LoginRequest) => {
      const response = await signIn(request)
      await openSession(response, setSession, setUser, queryClient)
      return response
    },
  })
}

/** Register and store the session (registration auto-signs the user in). */
export function useRegisterMutation() {
  const setSession = useAuthStore((state) => state.setSession)
  const setUser = useAuthStore((state) => state.setUser)
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (request: RegisterRequest) => {
      const response = await registerAccount(request)
      await openSession(response, setSession, setUser, queryClient)
      return response
    },
  })
}

/**
 * Revoke the refresh token server-side and drop the local session. The local
 * session clears regardless of the server answer — signing out must always
 * sign out locally.
 */
export function useLogoutMutation() {
  const clearSession = useAuthStore((state) => state.clearSession)
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (request: LogoutRequest) => signOut(request),
    onSettled: () => {
      clearSession()
      queryClient.removeQueries()
    },
  })
}

/** Ask the backend to email a 6-digit reset code (anti-enumeration: always succeeds). */
export function useForgotPasswordMutation() {
  return useMutation({
    mutationFn: async (request: ForgotPasswordRequest) =>
      sendPasswordResetCode(request),
  })
}

/** Consume the emailed 6-digit code and set a new password. */
export function useResetPasswordMutation() {
  return useMutation({
    mutationFn: async (request: ResetPasswordRequest) => resetPassword(request),
  })
}


/** Update the signed-in user's profile information (FR-03). */
export function useUpdateProfileMutation() {
  const setUser = useAuthStore((state) => state.setUser)

  return useMutation({
    mutationFn: async (request: { phoneNumber: string }) => {
      const profile = await updateProfile(request)
      setUser(profile)
      return profile
    },
  })
}

// ─── Queries ─────────────────────────────────────────────────────────────────

/**
 * The signed-in user's profile. Disabled while signed out; keeps the auth
 * store's user snapshot fresh (login responses carry no roles or phone).
 */
export function useProfileQuery() {
  const authenticated = useAuthStore(selectIsAuthenticated)
  const setUser = useAuthStore((state) => state.setUser)

  const query = useQuery({
    queryKey: authQueryKeys.profile(),
    queryFn: fetchUserProfile,
    enabled: authenticated,
    staleTime: 60_000,
  })

  useEffect(() => {
    if (query.data) setUser(query.data)
  }, [query.data, setUser])

  return query
}
