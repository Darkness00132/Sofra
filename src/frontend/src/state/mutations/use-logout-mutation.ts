import { useMutation, useQueryClient } from "@tanstack/react-query"
import { signOut } from "@/api/endpoints/auth.api"
import type { LogoutRequestDto } from "@/models/dto/auth.dto"
import { useAuthStore } from "@/state/stores/auth-store"

/**
 * Revoke the refresh token server-side and drop the local session. The local
 * session clears regardless of the server answer — signing out must always
 * sign out locally.
 */
export function useLogoutMutation() {
  const clearSession = useAuthStore((state) => state.clearSession)
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (request: LogoutRequestDto) => signOut(request),
    onSettled: () => {
      clearSession()
      queryClient.removeQueries()
    },
  })
}
