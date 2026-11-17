import { useMutation } from "@tanstack/react-query"
import { resetPassword } from "@/api/endpoints/auth.api"
import type { ResetPasswordRequestDto } from "@/models/dto/auth.dto"

/** Consume the emailed 6-digit code and set a new password. */
export function useResetPasswordMutation() {
  return useMutation({
    mutationFn: async (request: ResetPasswordRequestDto) =>
      resetPassword(request),
  })
}
