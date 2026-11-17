import { useMutation } from "@tanstack/react-query"
import { sendPasswordResetCode } from "@/api/endpoints/auth.api"
import type { ForgotPasswordRequestDto } from "@/models/dto/auth.dto"

/** Ask the backend to email a 6-digit reset code (anti-enumeration: always succeeds). */
export function useForgotPasswordMutation() {
  return useMutation({
    mutationFn: async (request: ForgotPasswordRequestDto) =>
      sendPasswordResetCode(request),
  })
}
