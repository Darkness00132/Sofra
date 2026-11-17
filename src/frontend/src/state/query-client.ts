import { QueryClient } from "@tanstack/react-query"
import { isApiError } from "@/api/client"

/**
 * Server-state defaults. API failures are intentional answers (validation,
 * auth, conflicts) — only transport failures and 5xx responses retry.
 */
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      retry: (failureCount, error) => {
        if (isApiError(error) && error.status >= 400 && error.status < 500)
          return false
        return failureCount < 2
      },
    },
    mutations: { retry: false },
  },
})
