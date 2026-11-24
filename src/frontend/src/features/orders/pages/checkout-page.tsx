import { Link } from "react-router-dom"
import { FormTextField } from "@/shared/components/form-text-field"
import { PageLoader } from "@/shared/components/page-loader"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useCheckout } from "../hooks/use-checkout"

/**
 * Checkout (FR-25…FR-31): reviews the exact charges for the current cart
 * (preview refreshes on fulfilment change), collects fulfilment + payment and
 * places the order.
 */
export function CheckoutPage() {
  const { form, charges, formError, isPlacing } = useCheckout(() => {
    // placement success — the hook navigates; nothing else to do here
  })

  return (
    <main className="mx-auto flex min-h-svh max-w-2xl flex-col gap-4 p-4">
      <h1 className="text-2xl font-semibold">Checkout</h1>

      <Card>
        <CardHeader>
          <CardTitle>Charges (FR-25, FR-27)</CardTitle>
          <CardDescription>
            Computed from your current cart — refreshes when you change the
            fulfilment method.
          </CardDescription>
        </CardHeader>
        <CardContent className="text-sm">
          {charges.isLoading ? (
            <PageLoader />
          ) : charges.isError ? (
            <p className="text-destructive">
              {(charges.error as Error).message}
            </p>
          ) : charges.data ? (
            <div className="flex flex-col gap-1">
              <p>
                <span className="text-muted-foreground">Subtotal: </span>
                {charges.data.subtotal}
              </p>
              <p>
                <span className="text-muted-foreground">Tax: </span>
                {charges.data.tax}
              </p>
              <p>
                <span className="text-muted-foreground">Delivery fee: </span>
                {charges.data.deliveryFee}
              </p>
              <p className="text-base font-medium">
                <span className="text-muted-foreground">Total: </span>
                {charges.data.total}
              </p>
            </div>
          ) : null}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Fulfilment & payment (FR-26, FR-27)</CardTitle>
          <CardDescription>
            Delivery requires an address; cash payments complete when the order
            does.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form
            onSubmit={(event) => {
              event.preventDefault()
              form.handleSubmit()
            }}
            className="flex flex-col gap-4"
          >
            {formError ? (
              <p className="text-sm text-destructive">{formError}</p>
            ) : null}
            <form.Field name="fulfilmentType">
              {(field) => (
                <div className="flex flex-col gap-2">
                  <label htmlFor={field.name}>Fulfilment</label>
                  <select
                    id={field.name}
                    className="rounded-md border bg-background px-3 py-2 text-sm"
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={(event) =>
                      field.handleChange(
                        event.target.value as "Pickup" | "Delivery"
                      )
                    }
                  >
                    <option value="Pickup">Pickup</option>
                    <option value="Delivery">Delivery</option>
                  </select>
                </div>
              )}
            </form.Field>
            <form.Field name="deliveryAddress">
              {(field) => (
                <FormTextField
                  id={field.name}
                  label="Delivery address"
                  value={field.state.value}
                  errors={field.state.meta.errors}
                  onBlur={field.handleBlur}
                  onChange={(value) => field.handleChange(value)}
                />
              )}
            </form.Field>
            <form.Field name="paymentMethod">
              {(field) => (
                <div className="flex flex-col gap-2">
                  <label htmlFor={field.name}>Payment</label>
                  <select
                    id={field.name}
                    className="rounded-md border bg-background px-3 py-2 text-sm"
                    value={field.state.value}
                    onBlur={field.handleBlur}
                    onChange={(event) =>
                      field.handleChange(
                        event.target.value as "CashOnDelivery" | "OnlinePayment"
                      )
                    }
                  >
                    <option value="CashOnDelivery">
                      Cash on delivery/pickup
                    </option>
                    <option value="OnlinePayment">Online payment</option>
                  </select>
                </div>
              )}
            </form.Field>
            <form.Subscribe selector={(state) => state.isSubmitting}>
              {(submitting) => (
                <Button type="submit" disabled={submitting || isPlacing}>
                  {submitting || isPlacing ? "Placing order…" : "Place order"}
                </Button>
              )}
            </form.Subscribe>
          </form>
        </CardContent>
        <CardFooter>
          <Link to="/cart" className="text-sm hover:underline">
            Back to cart
          </Link>
        </CardFooter>
      </Card>
    </main>
  )
}
