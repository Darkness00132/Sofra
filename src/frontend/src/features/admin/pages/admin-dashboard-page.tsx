import {
  Building2,
  ClipboardList,
  Clock3,
  FileClock,
  LayoutDashboard,
  ShieldAlert,
  Store,
  Users,
  Wallet,
} from "lucide-react"
import type { LucideIcon } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { CONSOLE_HERO } from "@/shared/lib/food-image"
import { useAdminDashboardQuery } from "../service/api"

/**
 * Platform-wide statistics for the administrator dashboard: restaurants,
 * pending applications, accounts and orders.
 */
export function AdminDashboardPage() {
  const dashboard = useAdminDashboardQuery()

  return (
    <div className="flex flex-col gap-6">
      <SurfaceHeader
        icon={LayoutDashboard}
        kicker="Administrator"
        title="Control overview"
        description="Restaurants, accounts, orders, and money across Sofra."
        image={CONSOLE_HERO}
      />

      {dashboard.isLoading ? (
        <PageLoader />
      ) : dashboard.isError ? (
        <p className="text-destructive">Could not load the dashboard.</p>
      ) : dashboard.data ? (
        <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
          <Stat icon={Store} label="Total restaurants" value={dashboard.data.totalRestaurants} />
          <Stat icon={Building2} label="Approved" value={dashboard.data.approvedRestaurants} />
          <Stat icon={ShieldAlert} label="Suspended" value={dashboard.data.suspendedRestaurants} />
          <Stat icon={FileClock} label="Pending applications" value={dashboard.data.pendingApplications} />
          <Stat icon={Users} label="Customers" value={dashboard.data.totalCustomers} />
          <Stat icon={ClipboardList} label="Orders" value={dashboard.data.totalOrders} />
          <Stat icon={Clock3} label="Pending orders" value={dashboard.data.pendingOrders} />
          <Stat icon={Wallet} label="Revenue" value={dashboard.data.totalRevenue} />
        </div>
      ) : null}
    </div>
  )
}

function Stat({
  icon: Icon,
  label,
  value,
}: {
  icon: LucideIcon
  label: string
  value: number
}) {
  return (
    <article className="flex items-start gap-3 rounded-2xl border bg-card p-4 shadow-sm">
      <span className="flex size-10 items-center justify-center rounded-xl bg-primary/10 text-primary">
        <Icon className="size-4" />
      </span>
      <div>
        <p className="text-sm text-muted-foreground">{label}</p>
        <p className="text-2xl font-semibold tabular-nums">{value}</p>
      </div>
    </article>
  )
}
