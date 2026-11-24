import { useState } from "react"
import { Link } from "react-router-dom"
import { ClipboardList } from "lucide-react"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { Skeleton } from "@/shared/components/ui/skeleton"
import { Alert, AlertDescription } from "@/shared/components/ui/alert"
import { Badge } from "@/shared/components/ui/badge"
import { Button } from "@/shared/components/ui/button"
import { Input } from "@/shared/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/components/ui/select"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/components/ui/table"
import { useAdminOrdersQuery } from "../service/api"
import type { AdminOrderFilter } from "../models/schemas"

const ORDER_STATUSES = [
  "Pending",
  "Accepted",
  "Preparing",
  "Ready",
  "Completed",
  "Rejected",
  "Cancelled",
]

export function AdminOrdersPage() {
  const [restaurantId, setRestaurantId] = useState<string | null>(null)
  const [status, setStatus] = useState<string | null>(null)
  const [pageNumber, setPageNumber] = useState(1)

  const filter: AdminOrderFilter = {
    restaurantId,
    customerId: null,
    status,
    from: null,
    to: null,
    pageNumber,
    pageSize: 10,
  }
  const orders = useAdminOrdersQuery(filter)

  return (
    <div className="mx-auto flex w-full max-w-6xl flex-col gap-6">
      <SurfaceHeader
        icon={ClipboardList}
        kicker="Administrator"
        title="Platform orders"
        description="Every order placed across Sofra, from pending to completed."
      />

      <div className="flex flex-col gap-3 sm:flex-row">
        <Select
          value={status ?? "all"}
          onValueChange={(value) => {
            setStatus(value === "all" ? null : value)
            setPageNumber(1)
          }}
        >
          <SelectTrigger className="w-full sm:w-48">
            <SelectValue placeholder="All statuses" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            {ORDER_STATUSES.map((s) => (
              <SelectItem key={s} value={s}>
                {s}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <Input
          className="sm:max-w-sm"
          placeholder="Restaurant id (guid)…"
          value={restaurantId ?? ""}
          onChange={(event) => {
            setRestaurantId(event.target.value || null)
            setPageNumber(1)
          }}
        />
      </div>

      {orders.isLoading ? (
        <div className="flex flex-col gap-2">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-12 w-full" />
          ))}
        </div>
      ) : orders.isError ? (
        <Alert variant="destructive">
          <AlertDescription>Could not load orders.</AlertDescription>
        </Alert>
      ) : (orders.data?.items.length ?? 0) === 0 ? (
        <p className="text-sm text-muted-foreground">No orders in this view.</p>
      ) : (
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Order</TableHead>
                <TableHead>Restaurant</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Fulfilment</TableHead>
                <TableHead>Payment</TableHead>
                <TableHead className="text-right">Total</TableHead>
                <TableHead>Created</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {orders.data!.items.map((order) => (
                <TableRow key={order.id}>
                  <TableCell>
                    <Link
                      to={`/admin/orders/${order.id}`}
                      className="font-medium hover:underline"
                    >
                      {order.id.slice(0, 8)}…
                    </Link>
                  </TableCell>
                  <TableCell>{order.restaurantName}</TableCell>
                  <TableCell>
                    <Badge variant="outline">{order.status}</Badge>
                  </TableCell>
                  <TableCell>{order.fulfilmentType}</TableCell>
                  <TableCell>{order.paymentMethod}</TableCell>
                  <TableCell className="text-right font-medium">
                    {order.total}
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {new Date(order.createdAt).toLocaleString()}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
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
    </div>
  )
}
