import { useState } from "react"
import { useForm } from "@tanstack/react-form"
import { useQuery } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { asApiError, routeApiErrorToForm } from "@/shared/lib/error-mapper"
import { previewOrder, usePlaceOrderMutation } from "../service/api"
import { checkoutFormSchema, type PlaceOrderResponse } from "../models/schemas"

const FIELD_NAMES = [
  "fulfilmentType",
  "paymentMethod",
  "deliveryAddress",
] as const

/**
 * View model for checkout (FR-25…FR-31): owns the fulfilment form, the live
 * charges preview for the current cart (an idempotent read — safe as a query)
 * and the placement mutation with server-error routing.
 */
export function useCheckout(onPlaced: (order: PlaceOrderResponse) => void) {
  const navigate = useNavigate()
  const place = usePlaceOrderMutation()
  const [formError, setFormError] = useState<string | null>(null)

  const form = useForm({
    defaultValues: {
      fulfilmentType: "Delivery" as "Pickup" | "Delivery",
      paymentMethod: "CashOnDelivery" as "CashOnDelivery" | "OnlinePayment",
      deliveryAddress: "",
    },
    validators: { onSubmit: checkoutFormSchema },
    onSubmit: async ({ value }) => {
      setFormError(null)
      try {
        const order = await place.mutateAsync({
          fulfilmentType: value.fulfilmentType,
          paymentMethod: value.paymentMethod,
          deliveryAddress:
            value.fulfilmentType === "Delivery" ? value.deliveryAddress : null,
        })
        onPlaced(order)
        navigate(`/orders/${order.id}`, { replace: true })
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

  // A read of the exact charges — re-computed when the fulfilment choice
  // changes. The delivery address does not change the charges.
  const charges = useQuery({
    queryKey: ["orders", "preview", form.state.values.fulfilmentType],
    queryFn: () =>
      previewOrder({
        fulfilmentType: form.state.values.fulfilmentType,
        deliveryAddress: null,
      }),
  })

  return { form, charges, formError, isPlacing: place.isPending }
}
