import { Link, useParams } from "react-router-dom"
import { PageLoader } from "@/shared/components/page-loader"
import { buttonVariants } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useAdminOrderDetails } from "../service/api"

/**
 * Full details of any platform order for administrator oversight (FR-45,
 * FR-51) — items, totals, payment and the status history.
 */
export function AdminOrderDetailsPage() {
  const { orderId = "" } = useParams()
  const order = useAdminOrderDetails(orderId)

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
          to="/admin/orders"
          className={buttonVariants({ variant: "outline" })}
        >
          Back to platform orders
        </Link>
      </main>
    )
  }

  const details = order.data

  return (
    <main className="mx-auto flex min-h-svh max-w-2xl flex-col gap-4 p-4">
      <Link
        to="/admin/orders"
        className={buttonVariants({ variant: "outline", className: "w-fit" })}
      >
        Back to platform orders
      </Link>

      <Card>
        <CardHeader>
          <CardTitle>
            Order {details.id.slice(0, 8)}… · {details.status}
          </CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-2 text-sm">
          <p>
            <span className="text-muted-foreground">Restaurant: </span>
            {details.restaurantName}
          </p>
          <p>
            <span className="text-muted-foreground">Payment: </span>
            {details.paymentMethod} ({details.paymentStatus ?? "unknown"})
          </p>
          <p>
            <span className="text-muted-foreground">Total: </span>
            {details.total}
          </p>
          <div className="mt-2 flex flex-col gap-1">
            {details.items.map((item) => (
              <p key={item.foodItemId}>
                {item.quantity} × {item.foodItemName} @ {item.unitPrice} ={" "}
                {item.lineTotal}
              </p>
            ))}
          </div>
          <div className="mt-2 flex flex-col gap-1">
            {details.statusHistory.map((entry, index) => (
              <p key={`${entry.status}-${index}`}>
                {new Date(entry.changedAt).toLocaleString()} · {entry.status}
                {entry.reason ? ` — ${entry.reason}` : ""}
              </p>
            ))}
          </div>
        </CardContent>
      </Card>
    </main>
  )
}
