import { useState } from "react"
import { Link } from "react-router-dom"
import { ClipboardList } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useRestaurantOrdersQuery } from "../service/api"

/**
 * The owner's restaurant orders (FR-32, FR-44): a status filter covers both
 * incoming orders and the order history — no separate history route.
 */
export function RestaurantOrdersPage() {
  const [status, setStatus] = useState<string | null>(null)
  const orders = useRestaurantOrdersQuery(status)

  return (
    <div className="mx-auto flex w-full max-w-2xl flex-col gap-4">
      <SurfaceHeader
        icon={ClipboardList}
        kicker="Restaurant"
        title="Incoming orders"
        description="Accept, prepare, and complete what customers just placed."
      />
      <div className="flex justify-end">
        <Link to="/restaurant/dashboard" className="text-sm hover:underline">
          Dashboard
        </Link>
      </div>

      <select
        className="rounded-md border bg-background px-3 py-2 text-sm"
        value={status ?? ""}
        onChange={(event) => setStatus(event.target.value || null)}
        aria-label="Filter by status"
      >
        <option value="">All statuses</option>
        <option value="Pending">Pending</option>
        <option value="Accepted">Accepted</option>
        <option value="Preparing">Preparing</option>
        <option value="Ready">Ready</option>
        <option value="Completed">Completed</option>
        <option value="Rejected">Rejected</option>
        <option value="Cancelled">Cancelled</option>
      </select>

      {orders.isLoading ? (
        <PageLoader />
      ) : orders.isError ? (
        <p className="text-destructive">Could not load orders.</p>
      ) : (orders.data?.items.length ?? 0) === 0 ? (
        <p className="text-muted-foreground">No orders in this view.</p>
      ) : (
        orders.data!.items.map((order) => (
          <Card key={order.id}>
            <CardHeader>
              <CardTitle>
                <Link to={`/restaurant/orders/${order.id}`} className="hover:underline">
                  Order {order.id.slice(0, 8)}…
                </Link>
              </CardTitle>
              <CardDescription>
                {order.status} · {order.fulfilmentType} ·{" "}
                {new Date(order.createdAt).toLocaleString()}
              </CardDescription>
            </CardHeader>
            <CardContent className="flex items-center justify-between text-sm">
              <span>{order.paymentMethod}</span>
              <span className="font-medium">Total: {order.total}</span>
            </CardContent>
          </Card>
        ))
      )}
    </div>
  )
}
