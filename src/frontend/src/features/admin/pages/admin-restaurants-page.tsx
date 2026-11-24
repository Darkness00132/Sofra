import { useState } from "react"
import { Building2 } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { SurfaceHeader } from "@/shared/components/surface-header"
import { Button } from "@/shared/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/components/ui/card"
import { Input } from "@/shared/components/ui/input"
import { useAdminRestaurantsQuery, useUpdateRestaurantStatusMutation } from "../service/api"

/**
 * Administrator policing of restaurants (FR-49): browse with filters and
 * suspend/reinstate — a suspended restaurant cannot accept orders (BR-09).
 */
export function AdminRestaurantsPage() {
  const [status, setStatus] = useState<string | null>(null)
  const [search, setSearch] = useState("")
  const restaurants = useAdminRestaurantsQuery(status, search)
  const updateStatus = useUpdateRestaurantStatusMutation()

  return (
    <div className="mx-auto flex w-full max-w-3xl flex-col gap-4">
      <SurfaceHeader
        icon={Building2}
        kicker="Administrator"
        title="Restaurants"
        description="Approve the ones that can sell, and suspend the ones that should not."
      />

      <div className="flex flex-col gap-2">
        <select
          className="rounded-md border bg-background px-3 py-2 text-sm"
          value={status ?? ""}
          onChange={(event) => setStatus(event.target.value || null)}
          aria-label="Filter by status"
        >
          <option value="">All statuses</option>
          <option value="Approved">Approved</option>
          <option value="Suspended">Suspended</option>
        </select>
        <Input
          placeholder="Search by name…"
          value={search}
          onChange={(event) => setSearch(event.target.value)}
        />
      </div>

      {restaurants.isLoading ? (
        <PageLoader />
      ) : restaurants.isError ? (
        <p className="text-destructive">Could not load restaurants.</p>
      ) : (restaurants.data?.length ?? 0) === 0 ? (
        <p className="text-muted-foreground">No restaurants in this view.</p>
      ) : (
        restaurants.data!.map((restaurant) => (
          <Card key={restaurant.id}>
            <CardHeader>
              <CardTitle>{restaurant.name}</CardTitle>
              <CardDescription>
                {restaurant.ownerEmail} · {restaurant.status} ·{" "}
                {restaurant.acceptingOrders
                  ? "accepting orders"
                  : "not accepting"}
              </CardDescription>
            </CardHeader>
            <CardContent className="flex gap-2">
              {restaurant.status === "Approved" ? (
                <Button
                  variant="destructive"
                  disabled={updateStatus.isPending}
                  onClick={() =>
                    updateStatus.mutate({
                      restaurantId: restaurant.id,
                      request: { status: "Suspended" },
                    })
                  }
                >
                  Suspend
                </Button>
              ) : null}
              {restaurant.status === "Suspended" ? (
                <Button
                  disabled={updateStatus.isPending}
                  onClick={() =>
                    updateStatus.mutate({
                      restaurantId: restaurant.id,
                      request: { status: "Approved" },
                    })
                  }
                >
                  Reinstate
                </Button>
              ) : null}
            </CardContent>
          </Card>
        ))
      )}
    </div>
  )
}
