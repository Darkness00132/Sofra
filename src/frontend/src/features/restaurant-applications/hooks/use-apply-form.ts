import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useNavigate } from "react-router-dom"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import { useApplyMutation } from "../service/api"
import { applyApplicationFormSchema } from "../models/schemas"

const FIELD_NAMES = [
  "restaurantName",
  "address",
  "phone",
  "description",
] as const

/**
 * View model for the application form: owns the TanStack form, validates with
 * the feature schema, routes server failures (duplicate pending application)
 * via the shared error mapper, and bounces to the tracking page on success.
 */
export function useApplyForm() {
  const navigate = useNavigate()
  const apply = useApplyMutation()
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm({
    defaultValues: {
      restaurantName: "",
      address: "",
      phone: "",
      description: "",
    },
    validators: { onSubmit: applyApplicationFormSchema },
    onSubmit: async ({ value }) => {
      setFormError(null)
      try {
        await apply.mutateAsync(value)
        navigate("/restaurants/my-applications", { replace: true })
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
