import { z } from "zod"

/**
 * The auth feature's single source of truth: wire payload schemas, response
 * schemas and the stricter UX-level form schemas — with z.infer types
 * exported alongside. No separate dto/domain/mapper files; add a transform
 * only when data genuinely needs reshaping (there is none today — the schema
 * shapes work for the UI and the store).
 *
 * Password rules mirror the backend Identity policy (minimum 6 characters).
 */
// ─── Request payloads (wire) ─────────────────────────────────────────────────

export const registerRequestSchema = z.object({
  email: z.string(),
  password: z.string(),
})

export const loginRequestSchema = z.object({
  email: z.string(),
  password: z.string(),
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

// ─── Response payloads (bare JSON, no envelope) ──────────────────────────────

export const loginResponseSchema = z.object({
  accessToken: z.string(),
  refreshToken: z.string(),
  accessTokenExpiresAtUtc: z.iso.datetime(),
  user: z.object({ id: z.guid(), email: z.string() }),
  emailConfirmationToken: z.string().nullable().optional(),
})

export const userProfileResponseSchema = z.object({
  id: z.guid(),
  email: z.string(),
  phoneNumber: z.string().nullable(),
  roles: z.array(z.string()),
})

// ─── Inferred contracts ──────────────────────────────────────────────────────

export type RegisterRequest = z.infer<typeof registerRequestSchema>
export type LoginRequest = z.infer<typeof loginRequestSchema>
export type LogoutRequest = z.infer<typeof logoutRequestSchema>
export type ForgotPasswordRequest = z.infer<typeof forgotPasswordRequestSchema>
export type ResetPasswordRequest = z.infer<typeof resetPasswordRequestSchema>
export type LoginResponse = z.infer<typeof loginResponseSchema>
export type UserProfile = z.infer<typeof userProfileResponseSchema>

// ─── Form schemas (stricter than the wire) ───────────────────────────────────

export const loginFormSchema = z.object({
  email: z.email("Enter a valid email address."),
  password: z.string().min(1, "Password is required."),
})

export const registerFormSchema = z
  .object({
    email: z.email("Enter a valid email address."),
    password: z.string().min(6, "Password must be at least 6 characters long."),
    confirmPassword: z.string().min(1, "Confirm the password."),
  })
  .refine((data) => data.password === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Passwords do not match.",
  })

export const forgotPasswordFormSchema = z.object({
  email: z.email("Enter a valid email address."),
})

export const updateProfileFormSchema = z.object({
  phoneNumber: z.string().min(1, "Phone number is required."),
})

export const resetPasswordFormSchema = z
  .object({
    email: z.email("Enter a valid email address."),
    otp: z.string().regex(/^\d{6}$/, "Enter the 6-digit code from the email."),
    newPassword: z
      .string()
      .min(6, "Password must be at least 6 characters long."),
    confirmPassword: z.string().min(1, "Confirm the password."),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    path: ["confirmPassword"],
    message: "Passwords do not match.",
  })
