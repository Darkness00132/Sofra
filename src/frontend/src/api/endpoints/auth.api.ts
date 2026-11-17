import { api } from "@/api/client"
import type {
  ForgotPasswordRequestDto,
  LoginRequestDto,
  LoginResponseDto,
  LogoutRequestDto,
  RefreshAccessTokenRequestDto,
  RegisterRequestDto,
  ResetPasswordRequestDto,
  UserProfileResponseDto,
} from "@/models/dto/auth.dto"
import {
  loginResponseSchema,
  userProfileResponseSchema,
} from "@/schemas/api/auth.schemas"

/**
 * Pure auth endpoint calls. Each response is parsed with its zod schema so a
 * contract breach surfaces at the boundary instead of deep inside the UI.
 * signOut returns 204 and sendPasswordResetCode 200 with an empty body — no
 * payload to parse.
 */

export async function registerAccount(
  request: RegisterRequestDto
): Promise<LoginResponseDto> {
  const response = await api.post("/auth/register", request)
  return loginResponseSchema.parse(response.data)
}

export async function signIn(
  request: LoginRequestDto
): Promise<LoginResponseDto> {
  const response = await api.post("/auth/login", request)
  return loginResponseSchema.parse(response.data)
}

export async function refreshSession(
  request: RefreshAccessTokenRequestDto
): Promise<LoginResponseDto> {
  const response = await api.post("/auth/refresh", request)
  return loginResponseSchema.parse(response.data)
}

export async function signOut(request: LogoutRequestDto): Promise<void> {
  await api.post("/auth/logout", request)
}

export async function sendPasswordResetCode(
  request: ForgotPasswordRequestDto
): Promise<void> {
  await api.post("/auth/forgot-password", request)
}

export async function resetPassword(
  request: ResetPasswordRequestDto
): Promise<void> {
  await api.post("/auth/reset-password", request)
}

export async function fetchUserProfile(): Promise<UserProfileResponseDto> {
  const response = await api.get("/auth/me")
  return userProfileResponseSchema.parse(response.data)
}
