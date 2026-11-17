import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useNavigate, useSearchParams } from "react-router-dom"
import {
  asApiError,
  isFormLevelError,
  serverFieldErrors,
} from "@/lib/form-errors"
import { resetPasswordFormSchema } from "@/schemas/forms/auth.form-schemas"
import { useResetPasswordMutation } from "@/state/mutations/use-reset-password-mutation"

const FIELD_NAMES = ["email", "otp", "newPassword"] as const

/**
 * View model for the reset-password page. The email arrives prefilled from
 * the forgot-password flow (?email=…); a successful reset returns to login.
 */
export function useResetPasswordForm() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const resetPassword = useResetPasswordMutation()
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm({
    defaultValues: {
      email: searchParams.get("email") ?? "",
      otp: "",
      newPassword: "",
      confirmPassword: "",
    },
    validators: { onSubmit: resetPasswordFormSchema },
    onSubmit: async ({ value }) => {
      setFormError(null)
      try {
        await resetPassword.mutateAsync({
          email: value.email,
          otp: value.otp,
          newPassword: value.newPassword,
        })
        navigate("/login", { replace: true })
      } catch (error) {
        const apiError = asApiError(error)
        if (!apiError) throw error
        // Bad codes and rejected passwords are envelope codes, not validation
        // maps — route each to the field it invalidates.
        if (apiError.code === "INVALID_RESET_CODE") {
          form.setErrorMap({
            onSubmit: { form: undefined, fields: { otp: apiError.detail } },
          })
        } else if (apiError.code === "PASSWORD_INVALID") {
          form.setErrorMap({
            onSubmit: {
              form: undefined,
              fields: { newPassword: apiError.detail },
            },
          })
        } else if (isFormLevelError(apiError)) {
          setFormError(apiError.detail)
        } else {
          form.setErrorMap({
            onSubmit: {
              form: undefined,
              fields: serverFieldErrors(apiError, FIELD_NAMES),
            },
          })
        }
      }
    },
  })

  return { form, formError }
}
