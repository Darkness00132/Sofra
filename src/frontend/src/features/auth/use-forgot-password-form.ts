import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { asApiError } from "@/lib/form-errors"
import { forgotPasswordFormSchema } from "@/schemas/forms/auth.form-schemas"
import { useForgotPasswordMutation } from "@/state/mutations/use-forgot-password-mutation"

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
        setFormError(apiError.detail)
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
