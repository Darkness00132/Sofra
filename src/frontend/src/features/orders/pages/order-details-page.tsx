import { Button, buttonVariants } from "@/shared/components/ui/button"
import { useState } from "react"
import { Link, useParams } from "react-router-dom"
import { PageLoader } from "@/shared/components/page-loader"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useCancelOrderMutation, useOrderDetailsQuery } from "../service/api"

/**
 * Full details of one customer order: items, totals, payment, delivery and the
 * status history (FR-36, FR-43) — plus the cancellation action (FR-39…FR-41).
 */
export function OrderDetailsPage() {
  const { orderId = "" } = useParams()
  const order = useOrderDetailsQuery(orderId)
  const cancel = useCancelOrderMutation()
  const [actionError, setActionError] = useState<string | null>(null)

  if (order.isLoading) {
    return (
      <main className="p-4">
        <PageLoader />
      </main>
    )
  }

  if (order.isError || !order.data) {
    return (
      <main className="p-4">
        <p className="text-destructive">The order was not found.</p>
        <Link to="/orders" className={buttonVariants({ variant: "outline" })}>
          Back to my orders
        </Link>
      </main>
    )
  }

  const details = order.data
  const cancellable = details.status === "Pending"

  return (
    <main className="mx-auto flex min-h-svh max-w-2xl flex-col gap-4 p-4">
      <Link
        to="/orders"
        className={buttonVariants({ variant: "outline", className: "w-fit" })}
      >
        Back to my orders
      </Link>

      <Card>
        <CardHeader>
          <CardTitle>
            Order {details.id.slice(0, 8)}… · {details.status}
          </CardTitle>
          <CardDescription>
            {details.restaurantName} · {details.fulfilmentType} ·{" "}
            {details.deliveryStatus}
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-2 text-sm">
          <p>
            <span className="text-muted-foreground">Payment: </span>
            {details.paymentMethod} ({details.paymentStatus ?? "unknown"})
          </p>
          {details.deliveryAddress ? (
            <p>
              <span className="text-muted-foreground">Deliver to: </span>
              {details.deliveryAddress}
            </p>
          ) : null}
          <p>
            <span className="text-muted-foreground">Placed: </span>
            {new Date(details.createdAt).toLocaleString()}
          </p>
          {details.completedAt ? (
            <p>
              <span className="text-muted-foreground">Completed: </span>
              {new Date(details.completedAt).toLocaleString()}
            </p>
          ) : null}
          <div className="mt-2 flex flex-col gap-1">
            {details.items.map((item) => (
              <p key={item.foodItemId}>
                {item.quantity} × {item.foodItemName} @ {item.unitPrice} ={" "}
                {item.lineTotal}
              </p>
            ))}
          </div>
          <p>
            <span className="text-muted-foreground">Subtotal: </span>
            {details.subtotal}
          </p>
          <p>
            <span className="text-muted-foreground">Tax: </span>
            {details.tax}
          </p>
          <p>
            <span className="text-muted-foreground">Delivery fee: </span>
            {details.deliveryFee}
          </p>
          <p className="text-base font-medium">
            <span className="text-muted-foreground">Total: </span>
            {details.total}
          </p>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Status history (FR-36)</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-1 text-sm">
          {details.statusHistory.map((entry, index) => (
            <p key={`${entry.status}-${index}`}>
              {new Date(entry.changedAt).toLocaleString()} · {entry.status}
              {entry.reason ? ` — ${entry.reason}` : ""}
            </p>
          ))}
        </CardContent>
      </Card>

      {cancellable ? (
        <Card>
          <CardHeader>
            <CardTitle>Cancel this order (FR-39)</CardTitle>
            <CardDescription>
              Pending orders can be cancelled. After the restaurant accepts,
              cancellation follows the restaurant's policy (FR-40).
            </CardDescription>
          </CardHeader>
          <CardContent className="flex flex-col gap-2">
            {actionError ? (
              <p className="text-sm text-destructive">{actionError}</p>
            ) : null}
            <Button
              variant="destructive"
              disabled={cancel.isPending}
              onClick={() =>
                cancel.mutate(
                  {
                    orderId: details.id,
                    request: { reason: "CustomerRequested", note: null },
                  },
                  {
                    onSuccess: () => setActionError(null),
                    onError: (error) => setActionError(error.message),
                  }
                )
              }
            >
              {cancel.isPending ? "Cancelling…" : "Cancel order"}
            </Button>
          </CardContent>
        </Card>
      ) : null}
    </main>
  )
}
