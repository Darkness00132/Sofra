import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useNavigate, useSearchParams } from "react-router-dom"
import {
  asApiError,
  isFormLevelError,
  serverFieldErrors,
} from "@/lib/form-errors"
import { loginFormSchema } from "@/schemas/forms/auth.form-schemas"
import { useLoginMutation } from "@/state/mutations/use-login-mutation"

const FIELD_NAMES = ["email", "password"] as const

/**
 * View model for the sign-in page: owns the TanStack form, runs the zod
 * schema on submit, routes server failures to fields or a form-level
 * message, and redirects to the page the user originally requested.
 */
export function useLoginForm() {
  const navigate = useNavigate()
  const [searchParams] = useSearchParams()
  const login = useLoginMutation()
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm({
    defaultValues: { email: "", password: "" },
    validators: { onSubmit: loginFormSchema },
    onSubmit: async ({ value }) => {
      setFormError(null)
      try {
        await login.mutateAsync(value)
        // Only follow in-app redirects — never off-origin values.
        const redirectTo = searchParams.get("redirectTo")
        const target =
          redirectTo &&
          redirectTo.startsWith("/") &&
          !redirectTo.startsWith("//")
            ? redirectTo
            : "/"
        navigate(target, { replace: true })
      } catch (error) {
        const apiError = asApiError(error)
        if (!apiError) throw error
        if (isFormLevelError(apiError)) {
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
