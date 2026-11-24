import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import { useForgotPasswordMutation } from "../service/api"
import { forgotPasswordFormSchema } from "../models/schemas"

const FIELD_NAMES = ["email"] as const

/**
 * View model for the forgot-password page. On success the page flips to the
 * "check your inbox" state (submittedEmail), which deep-links into the reset
 * form with the email prefilled.
 */
export function useForgotPasswordForm() {
  const forgotPassword = useForgotPasswordMutation()
  const [formError, setFormError] = useState<string | null>(null)
  const [submittedEmail, setSubmittedEmail] = useState<string | null>(null)

  const form = useForm({
    defaultValues: { email: "" },
    validators: { onSubmit: forgotPasswordFormSchema },
    onSubmit: async ({ value }) => {
      setFormError(null)
      try {
        await forgotPassword.mutateAsync(value)
        setSubmittedEmail(value.email)
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

  return {
    form,
    formError,
    submittedEmail,
    clearSubmittedEmail: () => setSubmittedEmail(null),
  }
}
