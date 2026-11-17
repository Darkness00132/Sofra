import type { z } from "zod"
import type {
  authUserResponseSchema,
  forgotPasswordRequestSchema,
  loginRequestSchema,
  loginResponseSchema,
  logoutRequestSchema,
  refreshAccessTokenRequestSchema,
  registerRequestSchema,
  resetPasswordRequestSchema,
  userProfileResponseSchema,
} from "@/schemas/api/auth.schemas"

/**
 * Wire contracts inferred from the zod schemas — the schemas stay the single
 * source of truth, so these types can never drift from runtime validation.
 */
export type RegisterRequestDto = z.infer<typeof registerRequestSchema>
export type LoginRequestDto = z.infer<typeof loginRequestSchema>
export type RefreshAccessTokenRequestDto = z.infer<
  typeof refreshAccessTokenRequestSchema
>
export type LogoutRequestDto = z.infer<typeof logoutRequestSchema>
export type ForgotPasswordRequestDto = z.infer<
  typeof forgotPasswordRequestSchema
>
export type ResetPasswordRequestDto = z.infer<typeof resetPasswordRequestSchema>
export type AuthUserDto = z.infer<typeof authUserResponseSchema>
export type LoginResponseDto = z.infer<typeof loginResponseSchema>
export type UserProfileResponseDto = z.infer<typeof userProfileResponseSchema>
