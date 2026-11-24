import { useState } from "react"
import { Link } from "react-router-dom"
import { MapPin, Phone, Search, Store } from "lucide-react"
import { PageLoader } from "@/shared/components/page-loader"
import { Badge } from "@/shared/components/ui/badge"
import { Button } from "@/shared/components/ui/button"
import { Input } from "@/shared/components/ui/input"
import { coverImage, MARKET_HERO } from "@/shared/lib/food-image"
import { useFoodCategoriesQuery, useRestaurantsQuery } from "../service/api"
import type { RestaurantBrowserFilter } from "../models/schemas"

/**
 * Public discovery: browse approved restaurants with search, category filter
 * and order-acceptance filter (FR-06, FR-08, FR-09). No sign-in required.
 */
export function RestaurantsPage() {
  const [search, setSearch] = useState("")
  const [categoryId, setCategoryId] = useState<string | null>(null)
  const [pageNumber, setPageNumber] = useState(1)

  const filter: RestaurantBrowserFilter = {
    search,
    categoryId,
    acceptingOrders: null,
    pageNumber,
    pageSize: 12,
  }

  const restaurants = useRestaurantsQuery(filter)
  const categories = useFoodCategoriesQuery(null)
  const pageCount = restaurants.data
    ? Math.max(1, Math.ceil(restaurants.data.totalCount / restaurants.data.pageSize))
    : 1

  return (
    <main>
      <section className="relative isolate overflow-hidden">
        <img
          src={MARKET_HERO}
          alt=""
          className="absolute inset-0 h-full w-full object-cover"
        />
        <div className="absolute inset-0 bg-linear-to-r from-black/75 via-black/55 to-black/20" />
        <div className="relative mx-auto flex max-w-6xl flex-col gap-4 px-4 py-16 text-white md:py-24">
          <p className="flex items-center gap-2 text-xs font-medium tracking-[0.16em] text-white/80 uppercase">
            <Store className="size-4" />
            Customer marketplace
          </p>
          <h1 className="max-w-xl text-4xl font-semibold tracking-tight md:text-5xl">
            Find a table’s worth of food, delivered.
          </h1>
          <p className="max-w-lg text-sm text-white/80 md:text-base">
            Browse restaurants that Sofra has approved, then order from the ones taking orders right now.
          </p>
        </div>
      </section>

      <section className="mx-auto flex max-w-6xl flex-col gap-6 px-4 py-8">
        <div className="flex flex-col gap-3 rounded-2xl border bg-card p-3 shadow-sm md:flex-row md:items-center">
          <div className="relative flex-1">
            <Search className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              className="pl-9"
              placeholder="Search by name…"
              value={search}
              onChange={(event) => {
                setSearch(event.target.value)
                setPageNumber(1)
              }}
            />
          </div>
          <div className="flex gap-2 overflow-x-auto">
            <Button
              size="sm"
              variant={categoryId === null ? "default" : "outline"}
              onClick={() => {
                setCategoryId(null)
                setPageNumber(1)
              }}
            >
              All
            </Button>
            {(categories.data?.items ?? []).map((category) => (
              <Button
                key={category.id}
                size="sm"
                variant={categoryId === category.id ? "default" : "outline"}
                onClick={() => {
                  setCategoryId(category.id)
                  setPageNumber(1)
                }}
              >
                {category.name}
              </Button>
            ))}
          </div>
        </div>

        {restaurants.isLoading ? (
          <PageLoader />
        ) : restaurants.isError ? (
          <p className="text-destructive">Could not load restaurants.</p>
        ) : (restaurants.data?.items.length ?? 0) === 0 ? (
          <p className="text-muted-foreground">No restaurants found.</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
            {restaurants.data!.items.map((restaurant) => (
              <Link
                key={restaurant.id}
                to={`/restaurants/${restaurant.id}`}
                className="group overflow-hidden rounded-2xl border bg-card shadow-sm transition-shadow hover:shadow-md"
              >
                <div className="relative h-44">
                  <img
                    src={coverImage(restaurant.imageUrl, restaurant.name)}
                    alt=""
                    className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-[1.03]"
                  />
                  <Badge className="absolute top-3 left-3">
                    {restaurant.acceptingOrders ? "Open" : "Paused"}
                  </Badge>
                </div>
                <div className="flex flex-col gap-2 p-4">
                  <h2 className="text-lg font-semibold tracking-tight">
                    {restaurant.name}
                  </h2>
                  <p className="line-clamp-2 text-sm text-muted-foreground">
                    {restaurant.description}
                  </p>
                  <p className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <MapPin className="size-3.5 shrink-0" />
                    <span className="truncate">{restaurant.address}</span>
                  </p>
                  <p className="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <Phone className="size-3.5 shrink-0" />
                    {restaurant.phone}
                  </p>
                </div>
              </Link>
            ))}
          </div>
        )}

        {restaurants.data ? (
          <div className="flex items-center justify-between">
            <Button
              variant="outline"
              disabled={pageNumber <= 1}
              onClick={() => setPageNumber((page) => page - 1)}
            >
              Previous
            </Button>
            <span className="text-sm text-muted-foreground">
              Page {restaurants.data.pageNumber} of {pageCount}
            </span>
            <Button
              variant="outline"
              disabled={pageNumber >= pageCount}
              onClick={() => setPageNumber((page) => page + 1)}
            >
              Next
            </Button>
          </div>
        ) : null}
      </section>
    </main>
  )
}
