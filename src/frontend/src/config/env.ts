const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL ?? "")
  .trim()
  .replace(/\/+$/, "")

/**
 * Runtime environment access in one place. In development `apiBaseUrl` is
 * empty: /api requests ride the Vite dev-server proxy (vite.config.ts), which
 * sidesteps CORS and the self-signed dev HTTPS certificate entirely. In
 * production it points at the deployed API origin (.env VITE_API_BASE_URL).
 */
export const env = {
  apiBaseUrl,
  apiPrefix: "/api/v1",
} as const
