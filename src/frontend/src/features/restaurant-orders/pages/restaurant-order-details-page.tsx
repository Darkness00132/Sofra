import { useState } from "react"
import { Link, useParams } from "react-router-dom"
import { buttonVariants } from "@/shared/components/ui/button"
import { PageLoader } from "@/shared/components/page-loader"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useRestaurantOrderQuery } from "../service/api"
import { useUpdateOrderStatusMutation } from "../service/api"
import { restaurantOrderStatuses } from "../models/schemas"

/**
 * Full details of one restaurant order plus the status transition control
 * (FR-33…FR-35, BR-06, BR-08): one status endpoint, domain guards decide.
 */
export function RestaurantOrderDetailsPage() {
  const { orderId = "" } = useParams()
  const order = useRestaurantOrderQuery(orderId)
  const updateStatus = useUpdateOrderStatusMutation()
  const [actionError, setActionError] = useState<string | null>(null)
  const [rejectionReason, setRejectionReason] = useState("")

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
        <Link
          to="/restaurant/orders"
          className={buttonVariants({ variant: "outline" })}
        >
          Back to orders
        </Link>
      </main>
    )
  }

  const details = order.data
  const showReasonInput = details.status === "Pending"

  const transition = (status: (typeof restaurantOrderStatuses)[number]) => {
    updateStatus.mutate(
      {
        orderId: details.id,
        request: {
          status,
          reason: status === "Rejected" ? rejectionReason || null : null,
        },
      },
      {
        onSuccess: () => setActionError(null),
        onError: (error) => setActionError(error.message),
      }
    )
  }

  return (
    <main className="mx-auto flex min-h-svh max-w-2xl flex-col gap-4 p-4">
      <Link
        to="/restaurant/orders"
        className={buttonVariants({ variant: "outline", className: "w-fit" })}
      >
        Back to restaurant orders
      </Link>

      <Card>
        <CardHeader>
          <CardTitle>
            Order {details.id.slice(0, 8)}… · {details.status}
          </CardTitle>
          <CardDescription>
            {details.fulfilmentType} · {details.paymentMethod} ·{" "}
            {details.deliveryStatus}
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-2 text-sm">
          <p>
            <span className="text-muted-foreground">Total: </span>
            {details.total}
          </p>
          {details.deliveryAddress ? (
            <p>
              <span className="text-muted-foreground">Deliver to: </span>
              {details.deliveryAddress}
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
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Status transitions (FR-33…FR-35)</CardTitle>
          <CardDescription>
            The domain validates every transition — invalid ones return 422
            (BR-06, BR-08).
          </CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-3">
          {actionError ? (
            <p className="text-sm text-destructive">{actionError}</p>
          ) : null}

          {showReasonInput && details.status === "Pending" ? (
            <input
              className="rounded-md border bg-background px-3 py-2 text-sm"
              placeholder="Rejection reason (required when rejecting)"
              value={rejectionReason}
              onChange={(event) => setRejectionReason(event.target.value)}
              aria-label="Rejection reason"
            />
          ) : null}

          <div className="flex flex-wrap gap-2">
            {details.status === "Pending" ? (
              <>
                <Button
                  disabled={updateStatus.isPending}
                  onClick={() => transition("Accepted")}
                >
                  Accept
                </Button>
                <Button
                  variant="destructive"
                  disabled={
                    updateStatus.isPending ||
                    (details.status === "Pending" && rejectionReason === "")
                  }
                  onClick={() => transition("Rejected")}
                >
                  Reject
                </Button>
              </>
            ) : null}
            {details.status === "Accepted" ? (
              <Button
                disabled={updateStatus.isPending}
                onClick={() => transition("Preparing")}
              >
                Start preparing
              </Button>
            ) : null}
            {details.status === "Preparing" ? (
              <Button
                disabled={updateStatus.isPending}
                onClick={() => transition("Ready")}
              >
                Mark ready
              </Button>
            ) : null}
            {details.status === "Ready" ? (
              <Button
                disabled={updateStatus.isPending}
                onClick={() => transition("Completed")}
              >
                Complete
              </Button>
            ) : null}
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Status history</CardTitle>
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
    </main>
  )
}
