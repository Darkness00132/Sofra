import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useNavigate } from "react-router-dom"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import { useRegisterMutation } from "../service/api"
import { registerFormSchema } from "../models/schemas"

const FIELD_NAMES = ["email", "password"] as const

/**
 * View model for the registration page. Registration auto-signs the user in,
 * so a successful submit lands on the home page with a live session.
 * EMAIL_ALREADY_REGISTERED is routed to the email field by the shared
 * error mapper's code registry.
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
