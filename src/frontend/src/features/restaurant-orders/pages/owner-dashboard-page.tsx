import type { ReactNode } from "react"
import { Link } from "react-router-dom"
import {
  CircleCheck,
  ClipboardList,
  Clock3,
  LayoutDashboard,
  Store,
  UtensilsCrossed,
  Wallet,
} from "lucide-react"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { buttonVariants } from "@/shared/components/ui/button"
import { Skeleton } from "@/shared/components/ui/skeleton"
import { Alert, AlertDescription } from "@/shared/components/ui/alert"
import { KITCHEN_HERO } from "@/shared/lib/food-image"
import { useOwnerDashboardQuery } from "../service/api"

export function OwnerDashboardPage() {
  const dashboard = useOwnerDashboardQuery()

  return (
    <div className="flex flex-col gap-6">
      <SurfaceHeader
        icon={LayoutDashboard}
        kicker="Restaurant"
        title="Today in the kitchen"
        description="Orders, revenue, and the tools that run your storefront."
        image={KITCHEN_HERO}
      />

      {dashboard.isLoading ? (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {Array.from({ length: 4 }).map((_, i) => (
            <Skeleton key={i} className="h-28 w-full" />
          ))}
        </div>
      ) : dashboard.isError ? (
        <Alert variant="destructive">
          <AlertDescription>Could not load the dashboard.</AlertDescription>
        </Alert>
      ) : dashboard.data ? (
        <>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <Stat icon={<ClipboardList className="size-4" />} label="Total orders" value={dashboard.data.totalOrders} />
            <Stat icon={<Clock3 className="size-4" />} label="Pending" value={dashboard.data.pendingOrders} />
            <Stat icon={<CircleCheck className="size-4" />} label="Completed" value={dashboard.data.completedOrders} />
            <Stat icon={<Wallet className="size-4" />} label="Revenue" value={dashboard.data.totalRevenue} />
          </div>

          <div className="grid gap-3 sm:grid-cols-3">
            <Link to="/restaurant/orders" className={actionClass}>
              <ClipboardList className="size-4" />
              Review orders
            </Link>
            <Link to="/menus" className={actionClass}>
              <UtensilsCrossed className="size-4" />
              Edit the menu
            </Link>
            <Link to="/restaurants/me" className={actionClass}>
              <Store className="size-4" />
              Storefront
            </Link>
          </div>
        </>
      ) : null}
    </div>
  )
}

const actionClass = buttonVariants({
  variant: "outline",
  className: "h-auto justify-start gap-2 rounded-2xl px-4 py-4",
})

function Stat({
  icon,
  label,
  value,
}: {
  icon: ReactNode
  label: string
  value: number
}) {
  return (
    <article className="rounded-2xl border bg-card p-4 shadow-sm">
      <p className="flex items-center gap-2 text-sm text-muted-foreground">
        <span className="text-primary">{icon}</span>
        {label}
      </p>
      <p className="mt-2 text-3xl font-semibold tabular-nums">{value}</p>
    </article>
  )
}
