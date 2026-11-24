import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useNavigate, useSearchParams } from "react-router-dom"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import { useResetPasswordMutation } from "../service/api"
import { resetPasswordFormSchema } from "../models/schemas"

const FIELD_NAMES = ["email", "otp", "newPassword"] as const

/**
 * View model for the reset-password page. The email arrives prefilled from
 * the forgot-password flow (?email=…); a successful reset returns to login.
 * INVALID_RESET_CODE and PASSWORD_INVALID are routed to their fields by the
 * shared error mapper's code registry.
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
        const { formError: serverError, fieldErrors } = routeApiErrorToForm(
          apiError,
          FIELD_NAMES
        )
        if (serverError) setFormError(serverError)
        else
          form.setErrorMap({
            onSubmit: { form: undefined, fields: fieldErrors },
          })
      }
    },
  })

  return { form, formError }
}
