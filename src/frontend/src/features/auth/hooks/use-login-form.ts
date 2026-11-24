import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useNavigate, useSearchParams } from "react-router-dom"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import { useLoginMutation } from "../service/api"
import { loginFormSchema } from "../models/schemas"

const FIELD_NAMES = ["email", "password"] as const

/**
 * View model for the sign-in page: owns the TanStack form, runs the zod
 * schema on submit, routes server failures via the shared error mapper, and
 * redirects to the page the user originally requested.
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
