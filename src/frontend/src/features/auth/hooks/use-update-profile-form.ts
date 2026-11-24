import { useEffect, useRef, useState } from "react"
import { useForm } from "@tanstack/react-form"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import { useProfileQuery, useUpdateProfileMutation } from "../service/api"
import { updateProfileFormSchema } from "../models/schemas"

const FIELD_NAMES = ["phoneNumber"] as const

/**
 * View model for the profile page: seeds the phone number from the loaded
 * profile, validates with the feature schema and routes server failures.
 */
export function useUpdateProfileForm() {
  const profile = useProfileQuery()
  const update = useUpdateProfileMutation()
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm({
    defaultValues: { phoneNumber: "" },
    validators: { onSubmit: updateProfileFormSchema },
    onSubmit: async ({ value }) => {
      setFormError(null)
      try {
        await update.mutateAsync(value)
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

  // Seed the form once the profile query has loaded.
  const seededRef = useRef(false)
  useEffect(() => {
    if (!seededRef.current && profile.data) {
      form.reset({ phoneNumber: profile.data.phoneNumber ?? "" })
      seededRef.current = true
    }
  }, [profile.data, form])

  return { form, formError, profile }
}

