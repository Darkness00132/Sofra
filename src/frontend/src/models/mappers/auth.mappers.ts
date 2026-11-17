import type {
  AuthUserDto,
  LoginResponseDto,
  UserProfileResponseDto,
} from "@/models/dto/auth.dto"
import type { AuthSession, AuthUser } from "@/models/domain/auth.models"

/** Login responses identify the user without roles or phone number. */
export function mapAuthUserDtoToUser(dto: AuthUserDto): AuthUser {
  return { id: dto.id, email: dto.email, roles: [] }
}

export function mapUserProfileDtoToUser(dto: UserProfileResponseDto): AuthUser {
  return {
    id: dto.id,
    email: dto.email,
    roles: dto.roles,
    phoneNumber: dto.phoneNumber ?? undefined,
  }
}

export function mapLoginResponseDtoToSession(
  dto: LoginResponseDto
): AuthSession {
  return {
    accessToken: dto.accessToken,
    refreshToken: dto.refreshToken,
    accessTokenExpiresAt: new Date(dto.accessTokenExpiresAtUtc),
    user: mapAuthUserDtoToUser(dto.user),
  }
}
