import { useMutation } from "@tanstack/react-query"
import { registerAccount } from "@/api/endpoints/auth.api"
import type { RegisterRequestDto } from "@/models/dto/auth.dto"
import { mapLoginResponseDtoToSession } from "@/models/mappers/auth.mappers"
import { useAuthStore } from "@/state/stores/auth-store"

/** Register and store the session (registration auto-signs the user in). */
export function useRegisterMutation() {
  const setSession = useAuthStore((state) => state.setSession)

  return useMutation({
    mutationFn: async (request: RegisterRequestDto) => {
      const session = mapLoginResponseDtoToSession(
        await registerAccount(request)
      )
      setSession(session)
      return session
    },
  })
}
