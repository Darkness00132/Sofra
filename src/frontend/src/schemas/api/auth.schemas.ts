import { z } from "zod"

// ─── Request payloads ────────────────────────────────────────────────────────
// Structural wire contracts (the backend binds these case-insensitively).
// Stricter UX-level rules live in schemas/forms.

export const registerRequestSchema = z.object({
  email: z.string(),
  password: z.string(),
})

export const loginRequestSchema = z.object({
  email: z.string(),
  password: z.string(),
})

export const refreshAccessTokenRequestSchema = z.object({
  refreshToken: z.string(),
})

export const logoutRequestSchema = z.object({
  refreshToken: z.string(),
})

export const forgotPasswordRequestSchema = z.object({
  email: z.string(),
})

export const resetPasswordRequestSchema = z.object({
  email: z.string(),
  otp: z.string(),
  newPassword: z.string(),
})

// ─── Response payloads ───────────────────────────────────────────────────────
// Successes are bare JSON — no envelope.

export const authUserResponseSchema = z.object({
  id: z.uuid(),
  email: z.string(),
})

export const loginResponseSchema = z.object({
  accessToken: z.string(),
  refreshToken: z.string(),
  accessTokenExpiresAtUtc: z.iso.datetime(),
  user: authUserResponseSchema,
})

export const userProfileResponseSchema = z.object({
  id: z.uuid(),
  email: z.string(),
  phoneNumber: z.string().nullable(),
  roles: z.array(z.string()),
})
