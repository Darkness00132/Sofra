import { useState } from "react"
import { Link } from "react-router-dom"
import { Receipt } from "lucide-react"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { ORDERS_HERO } from "@/shared/lib/food-image"
import { Badge } from "@/shared/components/ui/badge"
import { PageLoader } from "@/shared/components/page-loader"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { useMyOrdersQuery } from "../service/api"
import type { MyOrdersFilter } from "../models/schemas"

/**
 * The customer's order history with status/date filters and paging (FR-36,
 * FR-43). Status filters double as the history view (FR-44 client side).
 */
export function OrdersPage() {
  const [status, setStatus] = useState<string | null>(null)
  const [pageNumber, setPageNumber] = useState(1)

  const filter: MyOrdersFilter = {
    status,
    from: null,
    to: null,
    pageNumber,
    pageSize: 10,
  }
  const orders = useMyOrdersQuery(filter)

  return (
    <main className="mx-auto flex w-full max-w-2xl flex-col gap-4 px-4 py-8">
      <SurfaceHeader
        icon={Receipt}
        kicker="Customer"
        title="My orders"
        description="Track what you ordered and open any receipt."
        image={ORDERS_HERO}
      />
      <div className="flex justify-end">
        <Link to="/cart" className="text-sm hover:underline">
          Go to cart
        </Link>
      </div>

      <select
        className="rounded-md border bg-background px-3 py-2 text-sm"
        value={status ?? ""}
        onChange={(event) => {
          setStatus(event.target.value || null)
          setPageNumber(1)
        }}
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
        <p className="text-destructive">Could not load your orders.</p>
      ) : (orders.data?.items.length ?? 0) === 0 ? (
        <p className="text-muted-foreground">No orders found.</p>
      ) : (
        <div className="flex flex-col gap-3">
          {orders.data!.items.map((order) => (
            <Card key={order.id}>
              <CardHeader>
                <CardTitle>
                  <Link
                    to={`/orders/${order.id}`}
                    className="hover:underline"
                  >
                    Order {order.id.slice(0, 8)}…
                  </Link>
                </CardTitle>
                <CardDescription className="flex flex-wrap items-center gap-2">
                  <span>{order.restaurantName}</span>
                  <Badge variant="secondary">{order.status}</Badge>
                  <span>{new Date(order.createdAt).toLocaleString()}</span>
                </CardDescription>
              </CardHeader>
              <CardContent className="flex items-center justify-between text-sm">
                <span>
                  {order.fulfilmentType} · {order.paymentMethod}
                </span>
                <span className="font-medium">Total: {order.total}</span>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {orders.data ? (
        <div className="flex items-center justify-between">
          <Button
            variant="outline"
            disabled={pageNumber <= 1}
            onClick={() => setPageNumber((page) => page - 1)}
          >
            Previous
          </Button>
          <span className="text-sm text-muted-foreground">
            Page {orders.data.pageNumber} of{" "}
            {Math.max(1, Math.ceil(orders.data.totalCount / orders.data.pageSize))}
          </span>
          <Button
            variant="outline"
            disabled={
              pageNumber >=
              Math.ceil(orders.data.totalCount / orders.data.pageSize)
            }
            onClick={() => setPageNumber((page) => page + 1)}
          >
            Next
          </Button>
        </div>
      ) : null}
    </main>
  )
}
