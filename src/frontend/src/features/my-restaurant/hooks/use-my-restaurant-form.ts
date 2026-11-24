import { useEffect, useRef, useState } from "react"
import { useForm } from "@tanstack/react-form"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import {
  useMyRestaurantQuery,
  useUpdateMyRestaurantMutation,
} from "../service/api"
import { myRestaurantFormSchema } from "../models/schemas"

const FIELD_NAMES = ["name", "address", "phone", "description"] as const

/**
 * View model for the storefront info form: seeds defaults from the loaded
 * restaurant, validates with the feature schema and routes server failures.
 */
export function useMyRestaurantForm() {
  const restaurant = useMyRestaurantQuery()
  const update = useUpdateMyRestaurantMutation()
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm({
    defaultValues: {
      name: "",
      address: "",
      phone: "",
      description: "",
    },
    validators: { onSubmit: myRestaurantFormSchema },
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

  // Seed the form once the restaurant query has loaded (FR-12 edit flow).
  const seededRef = useRef(false)
  useEffect(() => {
    if (!seededRef.current && restaurant.data) {
      form.reset({
        name: restaurant.data.name,
        address: restaurant.data.address,
        phone: restaurant.data.phone,
        description: restaurant.data.description,
      })
      seededRef.current = true
    }
  }, [restaurant.data, form])

  return { form, formError, restaurant }
}
