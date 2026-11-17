import { useMutation } from "@tanstack/react-query"
import { signIn } from "@/api/endpoints/auth.api"
import type { LoginRequestDto } from "@/models/dto/auth.dto"
import { mapLoginResponseDtoToSession } from "@/models/mappers/auth.mappers"
import { useAuthStore } from "@/state/stores/auth-store"

/** Sign in and store the returned session. */
export function useLoginMutation() {
  const setSession = useAuthStore((state) => state.setSession)

  return useMutation({
    mutationFn: async (request: LoginRequestDto) => {
      const session = mapLoginResponseDtoToSession(await signIn(request))
      setSession(session)
      return session
    },
  })
}
