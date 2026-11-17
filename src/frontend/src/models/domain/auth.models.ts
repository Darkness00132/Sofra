/**
 * Clean auth shapes consumed by components — deliberately decoupled from the
 * wire (no DTO field names, no ISO expiry strings).
 */
export interface AuthUser {
  id: string
  email: string
  /** Populated from /auth/me; empty right after login (the login response carries none). */
  roles: string[]
  phoneNumber?: string
}

export interface AuthSession {
  accessToken: string
  refreshToken: string
  accessTokenExpiresAt: Date
  user: AuthUser
}
