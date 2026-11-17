import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useNavigate } from "react-router-dom"
import {
  asApiError,
  isFormLevelError,
  serverFieldErrors,
} from "@/lib/form-errors"
import { registerFormSchema } from "@/schemas/forms/auth.form-schemas"
import { useRegisterMutation } from "@/state/mutations/use-register-mutation"

const FIELD_NAMES = ["email", "password"] as const

/**
 * View model for the registration page. Registration auto-signs the user in,
 * so a successful submit lands on the home page with a live session.
 */
export function useRegisterForm() {
  const navigate = useNavigate()
  const register = useRegisterMutation()
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm({
    defaultValues: { email: "", password: "", confirmPassword: "" },
    validators: { onSubmit: registerFormSchema },
    onSubmit: async ({ value }) => {
      setFormError(null)
      try {
        await register.mutateAsync({
          email: value.email,
          password: value.password,
        })
        navigate("/", { replace: true })
      } catch (error) {
        const apiError = asApiError(error)
        if (!apiError) throw error
        // The backend reports duplicate accounts as an envelope code rather
        // than a validation map — route it to the email field anyway.
        if (apiError.code === "EMAIL_ALREADY_REGISTERED") {
          form.setErrorMap({
            onSubmit: { form: undefined, fields: { email: apiError.detail } },
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
