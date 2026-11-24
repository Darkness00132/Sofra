import { useState } from "react"
import { CreditCard } from "lucide-react"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { Skeleton } from "@/shared/components/ui/skeleton"
import { Alert, AlertDescription } from "@/shared/components/ui/alert"
import { Badge } from "@/shared/components/ui/badge"
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
import { useAdminPaymentsQuery } from "../service/api"

export function AdminPaymentsPage() {
  const [status, setStatus] = useState<string | null>(null)
  const [method, setMethod] = useState<string | null>(null)
  const payments = useAdminPaymentsQuery(status, method)

  return (
    <div className="mx-auto flex w-full max-w-5xl flex-col gap-6">
      <SurfaceHeader
        icon={CreditCard}
        kicker="Administrator"
        title="Payments"
        description="Cash and card records across every restaurant on Sofra."
      />

      <div className="flex flex-col gap-3 sm:flex-row">
        <Select
          value={status ?? "all"}
          onValueChange={(value) => setStatus(value === "all" ? null : value)}
        >
          <SelectTrigger className="w-full sm:w-48">
            <SelectValue placeholder="All statuses" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            <SelectItem value="Pending">Pending</SelectItem>
            <SelectItem value="Completed">Completed</SelectItem>
            <SelectItem value="Failed">Failed</SelectItem>
            <SelectItem value="Refunded">Refunded</SelectItem>
            <SelectItem value="Cancelled">Cancelled</SelectItem>
          </SelectContent>
        </Select>
        <Select
          value={method ?? "all"}
          onValueChange={(value) => setMethod(value === "all" ? null : value)}
        >
          <SelectTrigger className="w-full sm:w-48">
            <SelectValue placeholder="All methods" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All methods</SelectItem>
            <SelectItem value="CashOnDelivery">Cash on delivery</SelectItem>
            <SelectItem value="OnlinePayment">Online payment</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {payments.isLoading ? (
        <div className="flex flex-col gap-2">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-12 w-full" />
          ))}
        </div>
      ) : payments.isError ? (
        <Alert variant="destructive">
          <AlertDescription>Could not load payments.</AlertDescription>
        </Alert>
      ) : (payments.data?.items.length ?? 0) === 0 ? (
        <p className="text-sm text-muted-foreground">No payments in this view.</p>
      ) : (
        <div className="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Amount</TableHead>
                <TableHead>Method</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Order</TableHead>
                <TableHead>Paid at</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {payments.data!.items.map((payment) => (
                <TableRow key={payment.id}>
                  <TableCell className="font-medium">{payment.amount}</TableCell>
                  <TableCell>{payment.paymentMethod}</TableCell>
                  <TableCell>
                    <Badge variant="outline">{payment.status}</Badge>
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {payment.orderId.slice(0, 8)}…
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {payment.paidAt
                      ? new Date(payment.paidAt).toLocaleString()
                      : "—"}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}
    </div>
  )
}
