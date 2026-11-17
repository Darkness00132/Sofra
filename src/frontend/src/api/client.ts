import axios, { AxiosError } from "axios"
import type { InternalAxiosRequestConfig } from "axios"
import { env } from "@/config/env"
import { mapLoginResponseDtoToSession } from "@/models/mappers/auth.mappers"
import { loginResponseSchema } from "@/schemas/api/auth.schemas"
import { errorEnvelopeSchema } from "@/schemas/api/error.schemas"
import { useAuthStore } from "@/state/stores/auth-store"

/**
 * Application-level failure carrying the backend error envelope fields.
 * Every non-2xx answer (and transport failure) surfaces to callers as this
 * type — UI code never touches axios errors directly.
 */
export class ApiError extends Error {
  readonly status: number
  readonly code: string
  readonly detail: string
  readonly fieldErrors: Record<string, string[]> | undefined

  constructor(
    status: number,
    code: string,
    detail: string,
    fieldErrors?: Record<string, string[]>
  ) {
    super(detail)
    this.name = "ApiError"
    this.status = status
    this.code = code
    this.detail = detail
    this.fieldErrors = fieldErrors
  }
}

export function isApiError(error: unknown): error is ApiError {
  return error instanceof ApiError
}

function toApiError(
  error: unknown,
  fallbackCode = "REQUEST_FAILED",
  fallbackDetail = "The request failed. Please try again."
): ApiError {
  if (error instanceof ApiError) return error
  if (error instanceof AxiosError) {
    const status = error.response?.status ?? 0
    const envelope = errorEnvelopeSchema.safeParse(error.response?.data)
    if (envelope.success) {
      return new ApiError(
        status,
        envelope.data.code,
        envelope.data.detail,
        envelope.data.errors
      )
    }
    // A JWT-challenge 401 arrives with an empty body (no envelope); transport
    // failures have no response at all.
    if (status === 0) {
      return new ApiError(0, "NETWORK_ERROR", error.message || fallbackDetail)
    }
    const code = status === 401 ? "UNAUTHORIZED" : fallbackCode
    return new ApiError(
      status,
      code,
      error.response?.statusText || fallbackDetail
    )
  }
  return new ApiError(0, fallbackCode, fallbackDetail)
}

export const api = axios.create({
  baseURL: `${env.apiBaseUrl}${env.apiPrefix}`,
})

api.interceptors.request.use((config) => {
  const { accessToken } = useAuthStore.getState()
  if (accessToken) config.headers.set("Authorization", `Bearer ${accessToken}`)
  return config
})

// Auth endpoints manage the session themselves; refreshing on their 401s would loop.
const isAuthPath = (url?: string): boolean => Boolean(url?.startsWith("/auth/"))

interface RetriableRequestConfig extends InternalAxiosRequestConfig {
  _retried?: boolean
}

/** Single-flight guard so concurrent 401s share one refresh round-trip. */
let refreshInFlight: Promise<string> | null = null

/**
 * Rotate the refresh token into a new session. Uses a bare axios call so the
 * shared interceptors (which own this flow) are not re-entered. On failure
 * the local session is cleared exactly once for all waiting requests.
 */
async function refreshAccessToken(): Promise<string> {
  const { refreshToken, setSession, clearSession } = useAuthStore.getState()
  if (!refreshToken) {
    throw new ApiError(
      401,
      "NO_REFRESH_TOKEN",
      "Your session has expired. Please sign in again."
    )
  }
  try {
    const response = await axios.post(
      `${env.apiBaseUrl}${env.apiPrefix}/auth/refresh`,
      {
        refreshToken,
      }
    )
    const dto = loginResponseSchema.parse(response.data)
    setSession(mapLoginResponseDtoToSession(dto))
    return dto.accessToken
  } catch (error) {
    clearSession()
    throw toApiError(
      error,
      "SESSION_EXPIRED",
      "Your session has expired. Please sign in again."
    )
  }
}

api.interceptors.response.use(
  (response) => response,
  async (error: unknown) => {
    if (!(error instanceof AxiosError)) throw error

    const config = error.config as RetriableRequestConfig | undefined
    const status = error.response?.status

    if (
      status === 401 &&
      config &&
      !config._retried &&
      !isAuthPath(config.url)
    ) {
      // If the shared refresh fails, rejecting with its ApiError (session
      // already cleared) is exactly the outcome this handler wants.
      refreshInFlight ??= refreshAccessToken().finally(() => {
        refreshInFlight = null
      })
      const accessToken = await refreshInFlight

      config._retried = true
      config.headers.set("Authorization", `Bearer ${accessToken}`)
      return api.request(config)
    }

    throw toApiError(error)
  }
)
